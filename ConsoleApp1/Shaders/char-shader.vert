#version 330 core
uniform mat4 mvp;

in vec3 aPos;
in vec4 aColor;
in vec2 aTexCoord;

out vec2 texCoord;
out vec4 customColor;

void main(void)
{
    texCoord = aTexCoord;
    customColor = aColor;
    gl_Position = mvp * vec4(aPos, 1.0);
}