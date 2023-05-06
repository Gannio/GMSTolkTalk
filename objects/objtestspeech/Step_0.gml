/// @description Insert description here
// You can write your code in this editor


var box = getTextBoundingBox(text, fa_center, fa_top);
if point_in_rectangle(mouse_x,mouse_y,box[0],box[1],box[2],box[3]) && mouse_check_button_pressed(mb_left)
{
	TolkSilence();//Note: If silence is not used, the different dialogs will be queued up one after another.
	TolkSpeak(keyboard_string);
}
if (keyboard_check_pressed(vk_enter))
{
	keyboard_string += "\n";
}
