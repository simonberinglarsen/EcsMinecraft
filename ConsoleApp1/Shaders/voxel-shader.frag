#version 330 core
uniform sampler2DArray texture_array_sampler;

in vec4 vertexColor;
in vec3 texCoord;

out vec4 outputColor;

void main()
{
    vec4 texColor = texture(texture_array_sampler, texCoord);
    if(texColor.a <= 0) {
        discard;
    }
    outputColor = texColor * vertexColor;
} 