# Mummy's Curse

## Introduction

**Mummys' Curse** is a mixed reality multiplay escape room experience. Players need to observe carefully, look for clues, and work together to solve a series of puzzles and riddle in order to break the mummy's curse and escape from the tomb.

The game uses passthrough and occlusion to blend the virtual and real worlds, and utilizes a variety of physical props and sensors to enhance immersion.

<img width="685" height="970" alt="image" src="https://github.com/user-attachments/assets/ef4dfa7e-d328-4b71-9668-dd27558c30a6" />

[poster](https://drive.google.com/drive/folders/19A3F6wCXQ9Vh_v1lKL9-732FYGK7e6Bt)

## Design process

[**Design Folder**](https://drive.google.com/drive/folders/1O-wiWoeiYen7kSCbjZMcCvr8gcNXPC_h?usp=sharing)

### Phase 1: Mafia

[**Design Document**](https://docs.google.com/document/d/1Ehma-Y1a294HS1l4nNsgTa0KNRkZo7E6IM7gJ1hSYZs/edit?usp=sharing)

The very beginning idea of us. The idea is about assigning different roles to players while encouraging competition among them to partially replace cooperation, find the mafia hidden in the crowd or complete the conspiracy to win the game.

But we eventually abandoned the idea. The main reason was traditional mafia game require **at least 7 players** for a good gaming experience. In our case, the amount of HMDs and management of players can be a unignoreable problem. We tried designed a new rule based on traditional game specific for less players, but it is too complex for a 10-15 mins demonstration.

### Phase 2: Mummy's Curse 1.0

[**Design Document**](https://docs.google.com/document/d/1rfbI2MNi-GRsnRO2vtrxWCsdphHY01Io0in4PouiIhg/edit?usp=sharing)

The first version of our Mummy's Curse idea. The main feature is the whole game is divided into 9 different rooms, with different puzzles and riddles. We disigned 3 color-based puzzles and 3 riddles, if player give the wrong answer of the riddle, one of them will lose color, so they need to collabrate more with each other. Players can switch into different rooms through the minimap in the middle of room, specifically, when they press the button, the overlay wall and digital objects will be replaced, we want players can have the feeling of advanture like **Tomb Riders** through this.

We designed weight scale puzzle, mirror reflection beam puzzle, flame hand puzzle, and three egyption style riddle in this version, which you can find in document.

While after supervision, we decided to redesign this version, because for a mixed-reality experience, there are too less tangible interaction, basically we can did the same thing in only VR. (Although we still think it is good!)

### Phase 3: Mummy's Curse 2.0

[**Design Document**](https://docs.google.com/document/d/1rfbI2MNi-GRsnRO2vtrxWCsdphHY01Io0in4PouiIhg/edit?usp=sharing)

The current version of the game. We removed the minimap and room switch mechanism because it actually limit us to have tangible, physicial object in the game space. The weight scale and flame hand puzzles are inherited into this version, and be modified to be more tangible, for example, the weight of scale become physical props. The whole experience right now is more like an escape room experience, which players need to observe the whole room carefully, solve sequence of puzzles, and escape from the chamber of mummy.

## Features (TODO fill images)

1. Overlay

    Occlusion and Passthrough are used in project to blend the virtual and physical space. Player can see virtual object and real physical object at same time in correct geometry relationship.
   
    <img width="960" height="540" alt="image" src="https://github.com/user-attachments/assets/6575dfb8-e754-42d3-b640-c366a9d70797" />

3. MQTT Communication

    MQTT to facilitate communication between the sensors, actuators and the game session (hanlded by esp32). Specifically, this includes the values of hall effect sensors, signals to leds, language options and console monitoring.

    <img width="960" height="540" alt="image" src="https://github.com/user-attachments/assets/ecf599da-f959-4277-b972-ef046854905f" />

    <img width="960" height="540" alt="image" src="https://github.com/user-attachments/assets/51889c13-ed49-4319-8d16-11e0933cd6ea" />


4. Hall-Effect Sensor Based Props

    We designed a prop structure based on a hall effect sensor. Specifically, by controlling the distance between the magnet inside the prop and its bottom, the difference between reading value and baseline value can be used to define different prop types.

   <img width="960" height="540" alt="image" src="https://github.com/user-attachments/assets/12e001b8-eedc-4b63-9a88-361ca6e595ea" />

6. Sensor Calibration

    Because the magnetic field can vary depending on the environment and time, the baseline value of the hall effect sensor may differ. Therefore, the type of prop cannot be determined solely by a fixed interval; instead, the difference from the baseline should be used (as mentioned earlier). Furthermore, the baseline value needs to be recalibrated each time the game starts.

    ```csharp
    private IEnumerator Routine_Calibration()
    {
        yield return new WaitForSeconds(3f);
        var baseLine = (MQTTProcessor.Instance.Hall_0 + MQTTProcessor.Instance.Hall_1 + MQTTProcessor.Instance.Hall_2 + MQTTProcessor.Instance.Hall_3 + hall4) / 5f;
        MQTTProcessor.Instance.PublishMessage("MMYC/hall_base", Mathf.RoundToInt(baseLine).ToString());
        yield return null;
    }
    ```

7. QR Code Redirect

    We use the [trackable QR code](https://developers.meta.com/horizon/documentation/unity/unity-mr-utility-kit-qrcode-detection) feature of the Meta SDK to redirect the entire scene. When any player scans the corresponding QR code, the scene for all players will be redirected to correct position and direction, to make sure all players stay in same virtual space.

    [Source Code](Assets\Workspace\Scripts\QRRelocation.cs).

8. Multilingual

    We support both english and swedish version of text and voices. It controls by the MQTT signal: `MQTTProcessor.Instance.Language`

    <img width="960" height="540" alt="image" src="https://github.com/user-attachments/assets/dba3b952-971b-4efb-9988-519b2877242f" />

    <img width="960" height="540" alt="image" src="https://github.com/user-attachments/assets/c6476837-569b-4278-a165-83d2255b46eb" />

.
## Installation

Support devices: **Meta Quest 3/3s**

Unity Version: **6000.3.10.f1**

**Step 1:** There are two ways to get the installation file.

- Download from [release](https://github.com/curefate/MMYC/releases).
- `git clone https://github.com/curefate/MMYC.git`, clone this repository, open it in Unity using 6000.3.10.f1 or above, switch platform to Android, then compile.

**Step 2:** Install the .apk through Meta Quest Developer Hub, you will need a developer account to install unknown source application, see [details](https://developers.meta.com/horizon/documentation/native/android/mobile-device-setup/).

**Additioanl Steps:**

1. 3D print all props using models in design folder.
2. Upload arduino codes to your esp32, source code files are in /Assets/Workplace/Arduino.
3. Connect hall effect sensors to A0-A4. Leds are optional.

## Usage

### Puzzle 1 - Pillar Activation
Players begin the experience by moving a physical pillar prop into a specific play area. The position of the pillar is tracked in real time and used as the first interaction of the experience. Once the pillar reaches the correct location, the game activates and reveals the first passcode required to progress. This puzzle demonstrates the integration of physical object manipulation and mixed reality feedback.

<img width="960" height="540" alt="image" src="https://github.com/user-attachments/assets/2257aabb-654e-4d4b-a1ed-dde36af473d1" />

### Puzzle 2 - Lava bucket
Players use the previously discovered passcode to unlock a physical box containing a lava bucket prop. The bucket is tracked and overlaid with virtual effects using mixed reality passthrough. Interacting with the bucket grants each player a different virtual flame color, which is later used in the torch puzzle. This puzzle combines physical locks, tangible props, and virtual visual effects.

<img width="960" height="540" alt="image" src="https://github.com/user-attachments/assets/f208f270-a957-4c54-ada0-c7bd54cd7f90" />

### Puzzle 3 - Torches
Players must activate a series of wall-mounted torches using their assigned flame colors. Each player possesses a different flame color, encouraging collaboration and communication. The torches contain LEDs controlled through MQTT communication between the Unity application and ESP32 microcontrollers. When the correct player interacts with a torch, the corresponding LED lights up and part of a new passcode is revealed. This puzzle demonstrates real-time communication between mixed reality interactions and physical electronic devices.

<img width="960" height="540" alt="image" src="https://github.com/user-attachments/assets/9bed4aae-e84a-419f-ac89-c59bcc8e1c0f" />

### Puzzle 4 - Weight Scale
Players use a passcode obtained from the torch puzzle to unlock a second physical box containing 3D-printed weight pieces. Each piece contains real weight and represents a different value. Players place these pieces onto a physical scale and attempt to balance the heart and feather according to the puzzle requirements. Sensors embedded in the scale detect the selected weights and communicate the result back to the game. This puzzle combines tangible interaction, physical weight perception, and sensor-based input.

<img width="960" height="540" alt="image" src="https://github.com/user-attachments/assets/d230f525-6a93-49e2-ba25-85a47cfbc673" />

### Puzzle 5 - Riddle
The final challenge presents a riddle that can be experienced in either English or Swedish. Players must determine the correct answer and place the corresponding physical object onto a sensor-enabled pedestal. Sensors identify which object has been selected and validate the answer inside the game. Correct answers complete the experience, while incorrect answers trigger failure feedback. This puzzle demonstrates multilingual support, physical object recognition, and mixed reality storytelling.

<img width="960" height="540" alt="image" src="https://github.com/user-attachments/assets/0cdf61c2-49d0-4ac6-90b0-3f9c8e7578ae" />

## References

[Assets List](https://docs.google.com/document/d/1rfbI2MNi-GRsnRO2vtrxWCsdphHY01Io0in4PouiIhg/edit?tab=t.7qvqmomuc5aj)

[MQTT Tools Code](https://gitea.dsv.su.se/ExtralityLab/se.su.dsv.extralitylab.unity)

[Meta SDK](https://developers.meta.com/horizon/develop/unity/)

## Contributors

[Fernando Valcazara](fernandovalcazara@gmail.com)

[Johanna Källström](johannackallstrom@gmail.com)

[Li Zijie](curefate@outlook.com)

[Jasmine Shahnavazi](jasminesh31@gmail.com)
