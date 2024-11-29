# Unity Tools Project

This is a simple localization tool, which helps support multiple languages within a game.

# Steps to add new language

1. Go to Languages folder and right click. (Create -> Localization -> LanguageData)

2. Name it however you want, my test file is called jwalant, and add the asset to the localizationManager object in the scene

3. Click on the file you created and in the inspector enter debug mode by clicking the three dots as shown in the image 2.png
   Enter a language name and then click off of debug mode

4. Open the tool as shown in image 1, by clicking (Tools -> Localization Tool) at the top

5. Now that you have the tool open, add the MasterLanguage from the Assest folder (it is not in the lanaguages folder but in a director higer)
   This is to prevent the master lanague keys from being altered

6. You can now either add new keys which will update the master lanague and sync to every other language in the folder

7. Or you can modify existing languages by selecting the language from the dropdown and modifing it, deleting a key here will not affect any
   other languge, but will sync up again atfer you modify the master language and or click sync keys even without modifying the master language

Best way to use would be to only add keys to the master language and delete keys from the master language as you will know which languages need values add to them noted by [Missing Translation] in the values section of the pairs
