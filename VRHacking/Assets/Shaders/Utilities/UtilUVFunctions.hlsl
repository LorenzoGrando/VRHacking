#pragma once

//generated code of ShaderGraph node
float2 TransformToPolarCoordinates(float2 UV, float2 Center, float RadialScale, float LengthScale) {
    float2 delta = UV - Center;
    float radius = length(delta) * 2 * RadialScale;
    float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
    return float2(radius, angle);
}

//https://www.ronja-tutorials.com/post/047-invlerp_remap/
float invLerp(float from, float to, float value){
    return (value - from) / (to - from);
}

//https://www.ronja-tutorials.com/post/047-invlerp_remap/
float remap(float origFrom, float origTo, float targetFrom, float targetTo, float value){
    float rel = invLerp(origFrom, origTo, value);
    return lerp(targetFrom, targetTo, rel);
}