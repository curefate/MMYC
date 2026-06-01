# Mummy's Curse

## Introduction

**Mummys' Curse** is a mixed reality multiplay escape room experience. Players need to observe carefully, look for clues, and work together to solve a series of puzzles and a riddle in order to break the mummy's curse and escape from the tomb.

The game uses passthrough and occlusion to blend the virtual and the real world, and utilizes a variety of physical props and sensors to enhance immersion.

<img width="685" height="970" alt="image" src="https://github.com/user-attachments/assets/ef4dfa7e-d328-4b71-9668-dd27558c30a6" />

[poster](https://drive.google.com/drive/folders/19A3F6wCXQ9Vh_v1lKL9-732FYGK7e6Bt)

## Design process

[**Design Folder**](https://drive.google.com/drive/folders/1O-wiWoeiYen7kSCbjZMcCvr8gcNXPC_h?usp=sharing)

### Phase 1: Mafia

[**Design Document**](https://docs.google.com/document/d/1Ehma-Y1a294HS1l4nNsgTa0KNRkZo7E6IM7gJ1hSYZs/edit?usp=sharing)

The very beginning idea from our team. The concept was to assign different roles to players while encouraging competition among them to partially replace cooperation. Players would either find the mafia hidden in the crowd or complete the conspiracy to win the game.

However, we eventually abandoned the idea. The main reason was that a traditional mafia game requires **at least 7 players** to provide a good gameplay experience. In our case, the number of HMDs and the management of players could become a significant problem. We tried designing a new set of rules based on the traditional game for fewer players, but it was too complex for a 10–15 minute demonstration.

### Phase 2: Mummy's Curse 1.0

[**Design Document**](https://docs.google.com/document/d/1rfbI2MNi-GRsnRO2vtrxWCsdphHY01Io0in4PouiIhg/edit?usp=sharing)

The first version of our Mummy's Curse concept. The main feature was that the entire game was divided into 9 different rooms, each containing different puzzles and riddles. We designed 3 color-based puzzles and 3 riddles. If players gave the wrong answer to a riddle, one of them would lose their color vision, forcing the group to collaborate more closely. Players could switch between rooms through a minimap located in the center of the room. Specifically, when a button was pressed, the virtual walls and digital objects would be replaced, giving players a sense of adventure similar to Tomb Raider.

In this version, we designed a weight scale puzzle, a mirror reflection beam puzzle, a flame hand puzzle, and three Egyptian-style riddles, which can be found in the design document.

However, after supervision, we decided to redesign this version because, for a mixed reality experience, it contained too few tangible interactions. Essentially, most of the experience could have been achieved in a traditional VR environment. (Although we still believe it was a good concept!)

### Phase 3: Mummy's Curse 2.0

[**Design Document**](https://docs.google.com/document/d/1rfbI2MNi-GRsnRO2vtrxWCsdphHY01Io0in4PouiIhg/edit?usp=sharing)

The current version of the game. We removed the minimap and room-switching mechanism because it limited our ability to incorporate tangible physical objects into the play space. The weight scale and flame hand puzzles were carried over into this version and redesigned to be more tangible. For example, the scale weights became physical props that players can interact with directly.

The experience now resembles a mixed reality escape room, where players must carefully observe their surroundings, solve a sequence of interconnected puzzles, and ultimately escape from the mummy's chamber.

## Features

1. Overlay

    Occlusion and passthrough are used in the project to blend the virtual and physical environments. Players can see both virtual and real-world objects simultaneously while maintaining the correct spatial and geometric relationships between them. This allows virtual content to appear naturally integrated into the physical play space.
   
    <img width="960" height="540" alt="image" src="https://github.com/user-attachments/assets/6575dfb8-e754-42d3-b640-c366a9d70797" />

3. MQTT Communication

    MQTT is used to facilitate communication between the game session and the physical hardware components managed by ESP32 microcontrollers. Specifically, it is used to transmit hall-effect sensor readings, LED control signals, language selection options, and console monitoring information between the physical props and the Unity application.

    <img width="960" height="540" alt="image" src="https://github.com/user-attachments/assets/ecf599da-f959-4277-b972-ef046854905f" />

4. Hall-Effect Sensor Based Props

    We designed a prop identification system based on hall-effect sensors. By controlling the distance between a magnet inside each prop and the sensor located beneath it, different magnetic field readings can be generated. The difference between the measured value and a calibrated baseline value is then used to identify different prop types.

   <img width="960" height="540" alt="image" src="https://github.com/user-attachments/assets/12e001b8-eedc-4b63-9a88-361ca6e595ea" />

6. Sensor Calibration

    Because magnetic field readings can vary depending on environmental conditions and sensor drift over time, the baseline value of a hall-effect sensor may change. Therefore, prop identification cannot rely on fixed threshold values alone. Instead, the system uses the difference between the current reading and a calibrated baseline value, as described previously. To ensure reliable detection, the baseline is recalibrated automatically each time the game starts.

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

   We use the [trackable QR code](https://developers.meta.com/horizon/documentation/unity/unity-mr-utility-kit-qrcode-detection) feature provided by the Meta SDK to align the virtual environment with the physical play space. When any player scans the designated QR code, the scene is repositioned and reoriented for all connected players, ensuring that everyone shares the same virtual coordinate system and experiences the content in a consistent location.

    [Source Code](Assets\Workspace\Scripts\QRRelocation.cs).

9. Multilingual

    We support both English and Swedish versions of all text and voice content. The selected language is controlled through an MQTT signal and can be accessed within the game through `MQTTProcessor.Instance.Language`, allowing the experience to dynamically switch between supported languages.
   
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

1. 3D Print all props using models in design folder.
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
