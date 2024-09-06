import numpy as np
import matplotlib.pyplot as plt
import socket

from utils import DLT

def read_keypoints(filename):
    fin = open(filename, 'r')

    kpts = []
    while(True):
        line = fin.readline()
        if line == '': break

        line = line.split()
        line = [float(s) for s in line]

        line = np.reshape(line, (21, -1))
        kpts.append(line)

    kpts = np.array(kpts)
    return kpts


def visualize_3d(p3ds, pointsOfinterest):

    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    sock.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)

    direccion_broadcast = ('<broadcast>', 12345)  # Cambia el puerto si es necesario

    """Apply coordinate rotations to point z axis as up"""
    Rz = np.array(([[0., -1., 0.],
                    [1.,  0., 0.],
                    [0.,  0., 1.]]))
    Rx = np.array(([[1.,  0.,  0.],
                    [0., -1.,  0.],
                    [0.,  0., -1.]]))

    p3ds_rotated = []
    for frame in p3ds:
        frame_kpts_rotated = []
        for kpt in frame:
            kpt_rotated = Rz @ Rx @ kpt
            frame_kpts_rotated.append(kpt_rotated)
        p3ds_rotated.append(frame_kpts_rotated)

    pointsOfinterest_rotated = []
    for frame in pointsOfinterest:
        frame_kpts_rotated = []
        for kpt in frame:
            kpt_rotated = Rz @ Rx @ kpt
            frame_kpts_rotated.append(kpt_rotated)
        pointsOfinterest_rotated.append(frame_kpts_rotated)

    """this contains 3d points of each frame"""
    p3ds_rotated = np.array(p3ds_rotated)
    pointsOfinterest_rotated = np.array(pointsOfinterest_rotated)

    """Now visualize in 3D"""
    #thumb_f = [[0,1],[1,2],[2,3],[3,4]]
    #index_f = [[0,5],[5,6],[6,7],[7,8]]
    #middle_f = [[0,9],[9,10],[10,11],[11, 12]]
    #ring_f = [[0,13],[13,14],[14,15],[15,16]]
    #pinkie_f = [[0,17],[17,18],[18,19],[19,20]]
    #fingers = [pinkie_f, ring_f, middle_f, index_f, thumb_f]
    #fingers_colors = ['red', 'blue', 'green', 'black', 'orange']

    index_f = [[7,8]]
    #thumb_f = [[3,4]]
    fingers = [index_f]
    fingers_colors = ['red']

    from mpl_toolkits.mplot3d import Axes3D

    fig = plt.figure()
    ax = fig.add_subplot(111, projection='3d')

    tips = pointsOfinterest_rotated[:,8]

    vector1 = tips[0] - tips[1]
    vector2 = tips[2] - tips[1]

    magnitud = np.linalg.norm(vector1)
    magnitud2 = np.linalg.norm(vector2)

    cross = np.cross(vector1, vector2)

    newPoint = tips[0] + (cross/np.linalg.norm(cross))*3
    tips = np.vstack((tips,newPoint))

    newPoint = tips[1] + (cross/np.linalg.norm(cross))*3
    tips = np.vstack((tips,newPoint))

    newPoint = tips[2] + (cross/np.linalg.norm(cross))*3
    tips = np.vstack((tips,newPoint))

    newPoint = tips[3] + (cross/np.linalg.norm(cross))*3
    tips = np.vstack((tips,newPoint))

    #Obtenemos el centro
    newPoint = tips[1] + 0.5*(vector1 + vector2 + (cross/np.linalg.norm(cross))*3)
    tips = np.vstack((tips,newPoint))

    vectorX = vector1/magnitud
    vectorY = cross/np.linalg.norm(cross)
    vectorZ = vector2/magnitud2

    base_matrix = np.vstack([vector1, cross, vector2]).T

    for i, kpts3d in enumerate(p3ds_rotated):
        #if i%2 == 0: continue #skip every 2nd frame
        for finger, finger_color in zip(fingers, fingers_colors):
            for _c in finger:
                ax.plot(xs = [kpts3d[_c[0],0], kpts3d[_c[1],0]], ys = [kpts3d[_c[0],1], kpts3d[_c[1],1]], zs = [kpts3d[_c[0],2], kpts3d[_c[1],2]], linewidth = 4, c = finger_color)

            #if i%2 == 0: continue #skip every 2nd frame
        ax.scatter(tips[:,0], tips[:,1], tips[:,2])

        P = np.array([kpts3d[_c[1],0], kpts3d[_c[1],1], kpts3d[_c[1],2]])

        P_prime = P - newPoint

        coeficientes = np.linalg.solve(base_matrix, P_prime)
        coeficientes[1] = 4*coeficientes[1]

        #draw axes
        ax.plot(xs = [0,15], ys = [0,0], zs = [0,0], linewidth = 2, color = 'red')
        ax.plot(xs = [0,0], ys = [0,15], zs = [0,0], linewidth = 2, color = 'blue')
        ax.plot(xs = [0,0], ys = [0,0], zs = [0,15], linewidth = 2, color = 'black')

        message = "Index Right: "+str(coeficientes[0]) + ", " + str(coeficientes[1]) + ", " + str(coeficientes[2])

        sock.sendto(message.encode("utf-8"), direccion_broadcast)

        #ax.set_axis_off()
        ax.set_xticks([])
        ax.set_yticks([])
        ax.set_zticks([])

        ax.set_xlim3d(-20,20)
        ax.set_xlabel('x')
        ax.set_ylim3d(0,-35)
        ax.set_ylabel('y')
        ax.set_zlim3d(-100, -70)
        ax.set_zlabel('z')
        #ax.elev = 0.2*i
        #ax.azim = 0.2*i
        plt.savefig('figs/fig_' + str(i) + '.png')
        plt.pause(0.001)
        ax.cla()


if __name__ == '__main__':

    p3ds = read_keypoints('kpts_3d.dat')
    pointsOfinterest = read_keypoints('pointsOfInterest.dat')
    visualize_3d(p3ds, pointsOfinterest)
