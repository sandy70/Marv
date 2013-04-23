#pragma once

#using <mscorlib.dll>
#include <windows.h>

// #include <_vcclrit.h>

#pragma unmanaged

#ifdef SMILENET_PUBLIC
#include "smile.h"
#include "smilearn.h"
#else
#define SMILE_VC_NO_AUTOLINK
#define SMILEXML_VC_NO_AUTOLINK
#define SMILEARN_VC_NO_AUTOLINK
#include "../smile/smile.h"
#include "../smilearn/smilearn.h"
#endif

#pragma managed
