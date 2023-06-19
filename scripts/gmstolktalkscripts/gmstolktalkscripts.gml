// Script assets have changed for v2.3.0 see
// https://help.yoyogames.com/hc/en-us/articles/360005277377 for more information

/*
The following is a basic example of an override script for draw_text, 
to make all instances play text when a mouse is hovered over them.
It works by trying to create a bounding box for the text, based on the width and height, 
and outputting speech if the text is different from the global variable.
*/


global.TolkSpeech_LastSpoken = "";
#macro draw_text_original draw_text
//A way to access draw_text while avoiding the override.
#macro draw_text draw_text_speech
//Redirects draw_text's normal call to the function below.



function draw_text_speech(x,y,displayString, onClick = false, spok = displayString)
{
	
	
	var box = getTextBoundingBox(displayString, draw_get_halign(), draw_get_valign());
	
	
	if (point_in_rectangle(mouse_x,mouse_y,box[0],box[1],box[2],box[3]) && (!onClick || mouse_check_button_pressed(mb_left)))
	{
		draw_rectangle(box[0],box[1],box[2],box[3],c_blue);
		if (spok != global.TolkSpeech_LastSpoken)
		{
			//If mouse is inside the rectangle, and the current string does not match the last spoken string, output text on both speech and braille.
			global.TolkSpeech_LastSpoken = spok;
			TolkSilence();
			TolkOutput(spok);
		}
		
	}
	draw_text_original(x,y,displayString);
	
}
function getTextBoundingBox(textString, halign, valign)
{
	/*
	A means of getting the bounding box for general text ran by draw_text. Note that this will base things off the current font loaded, so you may need to adjust your code if you use multiple fonts.
	*/
	var bbL,bbT, bbR, bbB;
	var width = string_width(textString);
	var height = string_height(textString);
	switch (halign)
	{
		case fa_left:
			bbL = x;
			bbR = x + width;
		break;
		case fa_right:
			bbL = x - width;
			bbR = x;
		break;
		default:
			bbL = x-width*.5;
			bbR = x+width*.5;
		break;
	}
	switch (valign)
	{
		case fa_top:
			bbT = y;
			bbB = y + height;
		break;
		case fa_bottom:
			bbT = y - height;
			bbB = y;
		break;
		default:
			bbT = y-height*.5;
			bbB = y+height*.5;
		break;
	}
	return [bbL,bbT,bbR,bbB];
}
