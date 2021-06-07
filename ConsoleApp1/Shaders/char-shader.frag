#version 330
uniform sampler2D charTexture;

in vec2 texCoord;
in vec4 customColor;

out vec4 outputColor;

void main()
{
    vec4 texColor = texture(charTexture, texCoord);
    float alpha = customColor.a * texColor.a + (1 - customColor.a) * (1 - texColor.a);
    vec4 invTexColor = vec4(texColor.rgb, alpha);
    outputColor = vec4(customColor.rgb, 1.0) * invTexColor;
}