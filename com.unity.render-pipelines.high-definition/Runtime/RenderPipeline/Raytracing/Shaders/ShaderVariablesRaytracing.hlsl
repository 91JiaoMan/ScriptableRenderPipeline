#define RAYTRACING_OPAQUE_FLAG      0x01
#define RAYTRACING_TRANSPARENT_FLAG 0x02
#define RAYTRACING_CAST_SHADOW_FLAG 0x04
#define RAYTRACING_AMBIENT_OCLUSION_FLAG 0x08
#define RAYTRACING_REFLECTION_FLAG 0x10
#define RAYTRACING_GLOBAL_ILLUMINATION_FLAG 0x20
#define RAYTRACING_RECURSIVE_RAY_TRACING_FLAG 0x40
#define RAYTRACING_PATH_TRACING_FLAG 0x80

// The target acceleration acceleration structure should only be defined for non compute shaders
#ifndef SHADER_STAGE_COMPUTE
RaytracingAccelerationStructure         _RaytracingAccelerationStructure;
#endif
float                                   _RaytracingRayBias;
float                                   _RaytracingRayMaxLength;
int                                     _RaytracingNumSamples;
int                                     _RaytracingSampleIndex;
int                                     _RaytracingMinRecursion;
int                                     _RaytracingMaxRecursion;
float                                   _RaytracingIntensityClamp;
float                                   _RaytracingReflectionMaxDistance;
float                                   _RaytracingReflectionMinSmoothness;
int                                     _RaytracingIncludeSky;
int                                     _RaytracingFrameIndex;
float                                   _RaytracingPixelSpreadAngle;
int                                     _RayCountEnabled;
float                                   _RaytracingCameraNearPlane;
uint                                    _RaytracingDiffuseRay;
int                                     _RaytracingPreExposition;
RW_TEXTURE2D_X(uint4,                   _RayCountTexture);
