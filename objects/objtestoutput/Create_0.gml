//Run a simple suite of test functions.
TolkDebugSet(1);//Turn this off in release builds. Prints results of the API to better understand how each function works.
TolkTrySAPI(0);//Whether or not to include Micorsoft Speech API (SAPI) as an API to use.
TolkPreferSAPI(0);//Whether or not to prioritize SAPI over other screen readers.
TolkFallbackSet(1);//Whether or not to enable the fallback option if no other screen reader is installed.
TolkPowerOn();//Must be turned on before any output can be used.
TolkOutput("Hello World!");//Test output on both speech and braille.

text = "draw_text has been overwritten in scripts to automatically\nproduce output/draw a rectangle when the mouse hovers over text.\nYou can also set it to activate on-click with a fourth argument...";

//alarm[0] = 120;