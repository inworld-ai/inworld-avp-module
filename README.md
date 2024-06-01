# Inworld Polyspatial

Demonstration project showcasing the use of Inworld's Unity SDK with Polyspatial on the Apple Vision Pro

## Installation

1. Ensure you are working in Unity 2022.3.20f1 with the Vision Pro module installed and enabled
2. Create a new 3D Unity project using the default template (built-in render pipeline)
3. Install the following Unity packages:
   1. XR Plugin Management 4.4.1
   2. XR Hands 1.4.0
   3. Polyspatial, Polyspatial VisionOS and Polyspatial XR version - all version 1.1.4
4. Download and install the latest version of this project using the .unitypackage available from the Releases tab
5. Build and install the project to your Vision Pro

Note that this sample project includes a modified version of the Inworld SDK version 3.1

## Usage

Once you have built the project to your headset and have it opened, you should be greeted (after a brief loading period) with 

## Project Structure

The project is separated into four additively-loaded 'content scenes', and base scene responsible for initialization and holding objects that persist throughout the application's life.

The majority of the project's assets are contained within the `_LC` folder, with assets largely categorized by asset type (e.g. Prefabs, Models, Sprites) and then by which scene they are used in (Home, Fortune, Artefact or GameShow).

## Scene Structure - Changing Inworld Scenes

 It's important to note that in the current version of the Inworld SDK (version 3.2), a single InworldController component is permanently bound to a single Inworld scene.

This means that to change Inworld scene, you need to destroy the currently active Inworld Controller and create a new one with the scene ID that you wish to move to preconfigured in the inspector. In this sample project, this is achieved by using additive 'content' scenes that are loaded on top of the base scene.

The base scene contains the following GameObjects:

- **Base XR components**: including the AR Session, XR Origin, Polyspatial VolumeCamera, Gesture Recognition objects and the singleton controller for the touch interaction system
- **InworldConnectionRoutine**: which manages the initial load of the application
- **HatSwitcher**: which controls switching between the four content scenes based on the placement/removal of Innequin's hats
- **DebugSceneInterface**: a set of useful editor buttons to perform common functions difficult to do in the editor while not wearing a headset
- **SceneObjects common to all content scenes**: e.g. colliders matched to the application volume boundaries, the application UI (not counting the transcript canvases) and the grab handle used to move the application while in unbounded mode

Upon application start, **InworldConnectionRoutine** performs several functions to ensure the application starts up correctly (see the class itself for details) and then loads the first content scene.

Each of the content scene contains the following GameObjects:

-  A main scene objects which inherits from the [GdcScene](Assets/Inworld/Inworld.AVPModule/Scripts/Application/GdcScene.cs) class. This is the main class responsible for initializing the scene and ensuring that the Inworld Controller and Character have been successfully initialized. These classes also contain the interface layer between the scene logic and Inworld, as well as some other utility methods in some cases connecting the various elements of the scene together.    
See the GdcScene class for details on how to manage the process of unloading a currently-active InworldController/Character and loading a new one, as the process can be timing-sensitive.
- An InworldController and Character. Note that the InworldCharacter has a [DisableOnAwake](Assets/Inworld/Inworld.AVPModule/Scripts/Utils/DisableOnAwake.cs) component. This component has been added to the Script Execution Order settings for the project to ensure that its GameObject is disabled before the InworldCharacter has a chance to initialize (as when the scene first loads, the InworldController hasn't yet been activated). This was done to aid in scene composition - you can instead choose to remove this component and disable the GameObject manually (it will be enabled by the GdcScene GameObject)
- The transcript canvas that shows the real-time transcript of the player and innequin's messages. This object copies its visibility from the input field, as they are shown/hidden together
- Any scene assets and logic - this varies between scenes

## Useful Utility Classes

Several general-purpose components are included in this sample project that can be repurposed in your own projects:

-  **Gesture Recognition** - the classes [GestureRecognitionBase](Assets/Inworld/Inworld.AVPModule/Scripts/Interaction/GestureRecognizerBase.cs), [OneHandedGestureRecognizer](Assets/Inworld/Inworld.AVPModule/Scripts/Interaction/OneHandedGestureRecognizer.cs), and [LoveHeartGesture](Assets/Inworld/Inworld.AVPModule/Scripts/Interaction/LoveHeartGesture.cs) demonstrate how to use the XRHands Unity package to detect complex two-handed gestures within a unified framework as one-handed gesture.
-  **[InworldAsyncUtils](Assets/Inworld/Inworld.AVPModule/Scripts/Inworld/InworldAsyncUtils.cs)**: contains several static methods useful for common Inworld interactions that take place over several frames, as well as some shorthand methods to make code more concise (e.g. `InworldAsyncUtils.SendText("hello")` rather than `InworldController.Instance.SendText("hello", InworldController.CurrentCharacter.ID)`)
-  **[InworldCharacterEvents](Assets/Inworld/Inworld.AVPModule/Scripts/Inworld/InworldCharacterEvents.cs)**: contains a faster way to gain quick access to specific types of packets coming from the Inworld server. For example if you only want to gain access to `TextPacket`s you can simply write `InworldCharacterEvents.OnText += m => Debug.Log(m.text.text)`
-  **Interaction System**: A simple interaction system centered around Polyspatial interactions and magnetic snapping of objects onto target positions is included, contained within the [VisionOS](Assets/Inworld/Inworld.AVPModule/Scripts/VisionOS) directory. To use it, the scene requires a [TouchInteractionSystem](Assets/Inworld/Inworld.AVPModule/Scripts/VisionOS/TouchInteractionSystem.cs) component. From there objects can be made interactable by including a [TouchReceiver](Assets/Inworld/Inworld.AVPModule/Scripts/VisionOS/TouchReceiver.cs) component (or a component that inherits from it such as [Grabbable](Assets/Inworld/Inworld.AVPModule/Scripts/VisionOS/Grabbable.cs)). Add a [MagneticSnapTarget](Assets/Inworld/Inworld.AVPModule/Scripts/VisionOS/MagneticSnapTarget.cs) component to the scene to allow Grabbable objects to be snapped. See [RemovableHat](Assets/Inworld/Inworld.AVPModule/Scripts/Interaction/RemovableHat.cs) for an example of how to override the Grabbable class.

## Notes

#### Microphone Capture

This project includes Polyspatial microphone input to Inworld. To achieve this in a (somewhat) stable way, some modifications were made to [AudioCapture.cs](Assets/Inworld/Inworld.AI/Scripts/AudioCapture.cs), which is part of the Inworld SDK. The script works acceptable out of the box, but whenever the application loses focus (e.g. if the user minimizes it to the dock, or if the headset is removed), the unaltered AudioCapture class would never be able to regain microphone access.

The following changes were made:

- An OnAmplitude callback method was added to provide user-feedback on volume changes (executed within the `CalculateAmplitude` method)
- Property `IsRecording` added, which saves having to add compilation flags to the various parts of the class that check whether we are recording. This property is used to determine whether we need to reinitialize recording when we lose focus.
- We check whether the microphone is open when calling `OnRecording` (with the `IsRecording` property) and enable it if not. It's important to note that windowed Polyspatial apps do not lose focus when another app is interacted with, so this change is necessary to ensure the mic is open when we need it.

The most important class used to manage the microphone state is [AudioCaptureFocusManager.cs](Assets/Inworld/Inworld.AVPModule/Scripts/Inworld/AudioCaptureFocusManager.cs), which automatically enables/disables the AudioCapture component based on the changing focus state of the application.

#### Texture Preloading

In the current version of Polyspatial (VisionOS 1.1, Polyspatial 1.1.4), scripts which quickly change textures on materials (notably including changing Innequin's expression sprites) will sometimes show a magenta 'missing shader' material for a single frame the first time you use a sprite.

To address this, preloading scripts have been used to quickly load all necessary sprites on application load. This is done by simply assigning textures one frame at a time to a very tiny object in the scene. See The FacePreloader object (a child of the InworldConnectionRoutine GameObject in the Start scene). This technique is also used to preload the category textures for the LEDs in the game show scene.


#### Unbounded-mode scene movement

Moving the XROrigin in immersive-mode has no effect on the relative position of the scene objects to the tracking origin. Instead, the VolumeCamera must be moved. This changes where rendered objects appear relative to the camera, but also applies the inverse transform to all VisionOS-supplied transforms (e.g. camera position, interaction positions, etc).

As a result, moving and rotating the unbounded volume camera using these interaction positions can become quite complicated. See [VolumeGrabHandle.cs](Assets/Inworld/Inworld.AVPModule/Scripts/Interaction/VolumeGrabHandle.cs) for details on how this is done. Note that the class [VisionOsTransforms.cs](Assets/Inworld/Inworld.AVPModule/Scripts/VisionOS/VisionOsTransforms.cs) has been used to provide a more convenient way to transform VisionOS tracking-space transforms for the camera and hand positions into Unity world-space transforms. This is important for things like having Innequin look at the player or showing custom hand models.

## Known Issues

-  Due to an issue with Inworld's transcription service, spoken messages will be cut-off and incorrectly interpreted by Innequin
-  After changing characters several times, the microphone will fail to re-start, or all audio will be garbled and sped-up
-  Sometimes after changing to Immersive Mode, the scene will be invisible (probably very far away)
