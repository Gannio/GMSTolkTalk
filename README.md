# GMSTolkTalk
GMSTolkTalk DLL Library for Game Maker Studio that adds support for Speech/Braille interface. It uses [Tolk](https://github.com/ndarilek/tolk) as core for interfacing with a variety of screen readers, as well as implements Speech.Synthesis.

## License

Tolk is distributed under the GNU Lesser General Public License version 3 (LGPLv3).
Client libraries and headers are distributed under their own license (Namely, NVDA is distributed under the GNU Lesser General Public License version 2.1 (LGPLv2.1)).

## Supported screen readers

In addition to the screen readers supported by Tolk, GMSTolkTalk adds an additional fallback "driver" in the form of the Synthesis.Speech protocol. This should make it fully compatible with Windows devices that do not have other screen readers installed, or where the Microsoft Speech API (SAPI) is disabled. Note that Braille support is currently untested on all screen readers as well as the fallback driver, as I don't have a Braille device to test.

## Functions
The following functions are available in GMS. Functions marked with an asterisk (\*) only work with certain screen readers. Variables with turnOn can accept doubles or booleans in GMS.

### TolkPowerOn()
Required before most other methods. Load's Tolk's core. If not done, many functions below will return -1 or "NOT LOADED", depending on the return type. Returns 1 if successful, and 0 if it (somehow) fails. Note that it may take about 3 seconds for Tolk to fully load, during this time any call to speech may result in some lag until it is fully loaded.

### TolkPowerOff()
Unload's Tolk's core. Outside of the game_end function, this should only be necessary if you want to save on memory in a situation that otherwise does not require screen readers. Returns 1 if successful, and 0 if it (somehow) fails.

### TolkFallbackSet(turnOn)
Use to enable/disable the fallback Speech.Synthesis support.

### TolkDebugSet(turnOn)
Use to enable/disable debug printouts. Off by default.

### TolkTrySAPI(turnOn)
Has the Tolk core try using SAPI if it's available. 

### TolkPreferSAPI(turnOn)
Has the Tolk core prioritize SAPI over other methods.

### TolkGetScreenReader()
Returns a string containing the name of the currently running driver.
Screen Reader values are "JAWS", "NVDA", "System Access", "SAPI", "SuperNova", "Window-Eyes", "ZoomText". In the event the fallback is in use, "fallback" will be returned instead. If nothing is in use, "null" is returned (The string null, not actual null).

### TolkHasSpeech()
Returns whether the current driver has speech capability. Returns 1 if true, except for the fallback driver, which will return 2.

### TolkHasBraille()
Returns whether the current driver has braille capability. Returns 1 if true, except for the fallback driver, which will return 2. Note that braille support for the fallback driver is untested, and if it works likely outputs to both devices.

### TolkIsSpeaking()*
Returns whether or not the current driver is speaking or not. Note that this is only supported for SAPI, ZoomText, and the fallback driver. According to Tolk's source, other drivers only seem to have stubs that will return false.

### TolkSpeak(input)
### TolkBraille(input)
### TolkOutput(input)
Passes the string parameter to the appropriate driver to speak, pass to braille devices, or do both. Both is recommended unless you have a specific reason to have it seperated. If there are currently other lines being spoken by the driver, this string will be put in a queue, to come last in the sequence. Returns 1 on success with Tolk drivers, 2 on success^ with the fallback driver, and 3 if no driver was available.

^Success here is defined as having started the task that outputs the speech. This is done to ensure that GMS doesn't hang when running the speech, however it does mean that it's possible for some error to cause the driver to not run the speech, yet print a success. The same *may* be the case for the Tolk drivers as well, however I'd need to intentionally create a problem in those drivers in order to test it.

### TolkSilence()
Stops all speach output from the appropriate driver.

### TolkTest(input)
A preliminary, early test for basic speech output. Not recommend for actual use.

## Contributors

* [Gannio](https://github.com/Gannio)
* [Davy Kager](https://github.com/dkager), Leonard de Ruijter, Axel Vugts, QuentinC (Made the original Tolk core used in this project).
* [mijyuoon](https://github.com/mijyuoon), [Nnubes256](https://github.com/Nnubes256) (Fixed/explained some bugs with returning strings)