#version 330 core
uniform mat4 mvp;

in vec3 aPos;
in vec3 aCol;
  
out vec4 vertexColor;

void main()
{
    gl_Position = mvp * vec4(aPos, 1.0);
    vertexColor = vec4(aCol, 1.0);
}