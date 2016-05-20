# HQM-Icing
Interfaces with HockeyQuestionMark/HockeyEditor and HQMEditorDedicated to call Icing and print to HQM chat when icing is called.

# Usage
This project does not include HockeyEditor or HQMEditorDedicated. Please see https://github.com/HockeyQuestionMark to grab these DLLs for use with this application.

To utilize, make sure that HQMEditorDedicated or HockeyEditor is included (depending on if you want to use the local server or dedicated) and compile to an EXE in Visual Studio.

Make sure that the appropriate EXE file is running, then run the file you created. You should now have icing enabled.

# Support
Please direct any questions to https://www.reddit.com/u/xParabolax.

# Upcoming Features
1) Change faceoffs based on zone (left, right defensive zone). This would change puck location and skater location.

2) Enable 3 different options for icing based on configuration files: hybrid, no-touch, and touch icing. Currently no-touch is the only option.

3) Add more concise options for displaying the icing notifications.

4) Add countdown timer for faceoffs.

5) Check for invalid glitch shots that ricochet quickly and disable icing for those shots.

6) ~~Only enable icing when gameplay is active.~~ DONE

7) General code efficiency improvements.
