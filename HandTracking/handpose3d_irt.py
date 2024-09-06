import cv2 as cv
import mediapipe as mp
import numpy as np
import sys
import socket

from utils import DLT, get_projection_matrix, write_keypoints_to_disk

mp_drawing = mp.solutions.drawing_utils
mp_hands = mp.solutions.hands

frame_shape = [1080, 1920]#[720, 1280]

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

def run_mp(input_stream1, input_stream2, P0, P1):

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

    #input video stream
    cap0 = cv.VideoCapture(input_stream1)
    cap1 = cv.VideoCapture(input_stream2)
    caps = [cap0, cap1]

    #set camera resolution if using webcam to 1280x720. Any bigger will cause some lag for hand detection
    for cap in caps:
        cap.set(3, frame_shape[1])
        cap.set(4, frame_shape[0])

    #create hand keypoints detector object.
    hands0 = mp_hands.Hands(min_detection_confidence=0.5, max_num_hands =1, min_tracking_confidence=0.5)
    hands1 = mp_hands.Hands(min_detection_confidence=0.5, max_num_hands =1, min_tracking_confidence=0.5)

    #containers for detected keypoints for each camera
    kpts_cam0 = []
    kpts_cam1 = []
    kpts_3d = []

    pointsOfInterest = []

    calibrationPoints = read_keypoints('pointsOfInterest.dat')

    calibrationPoints_rotated = []
    for frame in calibrationPoints:
        frame_kpts_rotated = []
        for kpt in frame:
            kpt_rotated = Rz @ Rx @ kpt
            frame_kpts_rotated.append(kpt_rotated)
        calibrationPoints_rotated.append(frame_kpts_rotated)

    calibrationPoints_rotated = np.array(calibrationPoints_rotated)

    tips = calibrationPoints_rotated[:,8]

    vector1 = tips[0] - tips[1]
    vector2 = tips[2] - tips[1]

    magnitud = np.linalg.norm(vector1)
    magnitud2 = np.linalg.norm(vector2)

    cross = np.cross(vector1, vector2)

    newPoint = tips[0] + (cross/np.linalg.norm(cross))*1.75
    tips = np.vstack((tips,newPoint))

    newPoint = tips[1] + (cross/np.linalg.norm(cross))*1.75
    tips = np.vstack((tips,newPoint))

    newPoint = tips[2] + (cross/np.linalg.norm(cross))*1.75
    tips = np.vstack((tips,newPoint))

    newPoint = tips[3] + (cross/np.linalg.norm(cross))*1.75
    tips = np.vstack((tips,newPoint))

    #Obtenemos el centro
    newPoint = tips[1] + 0.5*(vector1 + vector2 + (cross/np.linalg.norm(cross))*1.75)
    tips = np.vstack((tips,newPoint))

    base_matrix = np.vstack([vector1, cross, vector2]).T

    while True:

        foundCam = True

        #read frames from stream
        ret0, frame0 = cap0.read()
        ret1, frame1 = cap1.read()

        if not ret0 or not ret1: break

        #crop to 720x720.
        #Note: camera calibration parameters are set to this resolution.If you change this, make sure to also change camera intrinsic parameters
        if frame0.shape[1] != 720:
            frame0 = frame0[:,frame_shape[1]//2 - frame_shape[0]//2:frame_shape[1]//2 + frame_shape[0]//2]
            frame1 = frame1[:,frame_shape[1]//2 - frame_shape[0]//2:frame_shape[1]//2 + frame_shape[0]//2]

        # the BGR image to RGB.
        frame0 = cv.cvtColor(frame0, cv.COLOR_BGR2RGB)
        frame1 = cv.cvtColor(frame1, cv.COLOR_BGR2RGB)

        # To improve performance, optionally mark the image as not writeable to
        # pass by reference.
        frame0.flags.writeable = False
        frame1.flags.writeable = False
        results0 = hands0.process(frame0)
        results1 = hands1.process(frame1)

        #prepare list of hand keypoints of this frame
        #frame0 kpts
        frame0_keypoints = []
        if results0.multi_hand_landmarks:
            for hand_landmarks in results0.multi_hand_landmarks:
                for p in range(21):
                    #print(p, ':', hand_landmarks.landmark[p].x, hand_landmarks.landmark[p].y)
                    pxl_x = int(round(frame0.shape[1]*hand_landmarks.landmark[p].x))
                    pxl_y = int(round(frame0.shape[0]*hand_landmarks.landmark[p].y))
                    kpts = [pxl_x, pxl_y]
                    frame0_keypoints.append(kpts)

        #no keypoints found in frame:
        else:
            #if no keypoints are found, simply fill the frame data with [-1,-1] for each kpt
            frame0_keypoints = [[-1, -1]]*21
            foundCam = False

        kpts_cam0.append(frame0_keypoints)

        #frame1 kpts
        frame1_keypoints = []
        if results1.multi_hand_landmarks:
            for hand_landmarks in results1.multi_hand_landmarks:
                for p in range(21):
                    #print(p, ':', hand_landmarks.landmark[p].x, hand_landmarks.landmark[p].y)
                    pxl_x = int(round(frame1.shape[1]*hand_landmarks.landmark[p].x))
                    pxl_y = int(round(frame1.shape[0]*hand_landmarks.landmark[p].y))
                    kpts = [pxl_x, pxl_y]
                    frame1_keypoints.append(kpts)

        else:
            #if no keypoints are found, simply fill the frame data with [-1,-1] for each kpt
            frame1_keypoints = [[-1, -1]]*21
            foundCam = False

        #update keypoints container
        kpts_cam1.append(frame1_keypoints)


        #calculate 3d position
        frame_p3ds = []
        for uv1, uv2 in zip(frame0_keypoints, frame1_keypoints):
            if uv1[0] == -1 or uv2[0] == -1:
                _p3d = [-1, -1, -1]
            else:
                _p3d = DLT(P0, P1, uv1, uv2) #calculate 3d position of keypoint
            frame_p3ds.append(_p3d)

        frame_kpts_rotated = []
        for kpt in frame_p3ds:
            kpt_rotated = Rz @ Rx @ kpt
            frame_kpts_rotated.append(kpt_rotated)



        '''
        This contains the 3d position of each keypoint in current frame.
        For real time application, this is what you want.
        '''
        frame_p3ds = np.array(frame_kpts_rotated).reshape((21, 3))
        #kpts_3d.append(frame_p3ds)

        P = np.array([frame_p3ds[8,0], frame_p3ds[8,1], frame_p3ds[8,2]])

        P_prime = P - newPoint

        coeficientes = np.linalg.solve(base_matrix, P_prime)
        coeficientes[1] = -8*2.5*coeficientes[1]

        message = "Index Right: "+str(coeficientes[0]) + ", " + str(coeficientes[1]) + ", " + str(-coeficientes[2])

        #if(foundCam):
        sock.sendto(message.encode("utf-8"), direccion_broadcast)

        P = np.array([frame_p3ds[4,0], frame_p3ds[4,1], frame_p3ds[4,2]])

        P_prime = P - newPoint

        coeficientes = np.linalg.solve(base_matrix, P_prime)
        coeficientes[1] = -8*2.5*coeficientes[1]

        message = "Thumb Right: "+str(coeficientes[0]) + ", " + str(coeficientes[1]) + ", " + str(-coeficientes[2])

        #if(foundCam):
        sock.sendto(message.encode("utf-8"), direccion_broadcast)

        # Draw the hand annotations on the image.
        frame0.flags.writeable = True
        frame1.flags.writeable = True
        frame0 = cv.cvtColor(frame0, cv.COLOR_RGB2BGR)
        frame1 = cv.cvtColor(frame1, cv.COLOR_RGB2BGR)

        if results0.multi_hand_landmarks:
          for hand_landmarks in results0.multi_hand_landmarks:
            mp_drawing.draw_landmarks(frame0, hand_landmarks, mp_hands.HAND_CONNECTIONS)

        if results1.multi_hand_landmarks:
          for hand_landmarks in results1.multi_hand_landmarks:
            mp_drawing.draw_landmarks(frame1, hand_landmarks, mp_hands.HAND_CONNECTIONS)
        cv.imshow('cam1', frame1)
        cv.imshow('cam0', frame0)

        k = cv.waitKey(1)
        if k & 0xFF == 27: break #27 is ESC key.

        #if k & 0xFF == 32: 
        #  pointsOfInterest.append(frame_p3ds)


    cv.destroyAllWindows()
    for cap in caps:
        cap.release()

    return np.array(kpts_cam0), np.array(kpts_cam1), np.array(kpts_3d), np.array(pointsOfInterest)

if __name__ == '__main__':

    input_stream1 = 'media/cam0_test.mp4'
    input_stream2 = 'media/cam1_test.mp4'

    if len(sys.argv) == 3:
        input_stream1 = int(sys.argv[1])
        input_stream2 = int(sys.argv[2])

    #projection matrices
    P0 = get_projection_matrix(0)
    P1 = get_projection_matrix(1)

    kpts_cam0, kpts_cam1, kpts_3d, pointsOfInterest = run_mp(input_stream1, input_stream2, P0, P1)

    #this will create keypoints file in current working folder
    write_keypoints_to_disk('kpts_cam0.dat', kpts_cam0)
    write_keypoints_to_disk('kpts_cam1.dat', kpts_cam1)
    write_keypoints_to_disk('kpts_3d.dat', kpts_3d)
    #write_keypoints_to_disk('pointsOfInterest.dat', pointsOfInterest)
