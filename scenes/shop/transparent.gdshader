shader_type spatial;
render_mode blend_mix, depth_draw_opaque;

uniform vec3 albedo : source_color = vec3(0.0, 0.5, 1.0);
uniform float alpha_value : hint_range(0.0, 1.0) = 0.5;

void fragment() {
	ALBEDO = albedo;
	ALPHA = alpha_value;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
