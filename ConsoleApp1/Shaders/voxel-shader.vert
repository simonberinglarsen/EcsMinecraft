#version 330 core
uniform mat4 mvp;
uniform float levelOfSunlight;

in vec3 aPos;
in vec3 aCol;
in float aLight;
in float aSkyLight;
in vec3 aTexCoord;

out vec4 vertexColor;
out vec3 texCoord;

void main()
{
    texCoord = aTexCoord;
    gl_Position = mvp * vec4(aPos, 1.0);
    vertexColor = vec4(aCol * max(aSkyLight * levelOfSunlight * 0.9 + 0.1, aLight), 1.0);
}