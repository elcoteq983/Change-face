   M S B A M L   `   `   `  ����M  IKinectFaceTracking, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:KinectFaceTrackingKinectFaceTracking  5   	   #    KinectFaceTracking.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   2local clr-namespace:KinectFaceTracking     5   	   -   5          � Title$  FaceTracking��5   	   $	��735��5   	   $	��770��6    � WindowStyle$
 None=�5	   	    � WindowState$ 	Maximized=�5
   	    � 	Resources 5         MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush �� 
TargetType �� Property �� Value��%�   &	        &	 #     &	 F     '�� i     �� 5   
   $ 	#ff6e6e6e��62   �� 5   
   $ 	#ff52318f��64   �� 5   
   $ 	#ff00BCF2��62   �� 5   
   " ��6   .�5      ��  �@J�SnapsToDevicePixels . 64   5      5      .��5      � -   5        $��
layoutGrid��5      $��
10 0 10 10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   ]� $��0��6   $��0 0 0 20q�6!   5      (� 5   
   	 (� Stretch$	 Uniform=�6   $��Center=�6$   $��1,0,-1,0q�6A   $��1��6S   .��5      � -   5        $��MainGrid��5      $	��640��6#   $	��480��6/   .+�5      �� -   5        $��
ColorImage��5      "   KinectFaceTracking.FaceImage  5      -   5        
  Name$
 faceTrackingViewer��5   "   $��0,2,0,-2q�6>   �� 5       -   5        $��image1��5       $
��Fill=�6&   $3��-/KinectFaceTracking;component/image/face3.png��65   $y�Hidden=�6l   $��1,0,559,390q�6�   �� 5!      -   5        $��image2��5!      $
��Fill=�6&   $3��-/KinectFaceTracking;component/image/face7.png��65   $y�Hidden=�6l   $��1,0,559,390q�6�   �� 5"      -   5        $��label1��5"   b   $��Label��6   $��55��6(   $
��Left=�64   $��	15,14,0,0q�6O   $	��Top=�6p   $	��126��6�   $��24(�6�   �� 5#      -	   5        $��Mybackground��5#   @   $	��506��6   $
��Left=�6%   $
��Fill=�6T   $	��Top=�6c   $	��640��6{   $@��:/KinectFaceTracking;component/bin/Debug/background/bk1.png��6�   $��	0,-23,0,0q�6�   5$      5%      5&      5'      