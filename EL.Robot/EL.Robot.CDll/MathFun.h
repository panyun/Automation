#pragma once
#ifdef __DLLEXPORT
#define __DLL_EXP _declspec(dllexport) 
#else
#define __DLL_EXP _declspec(dllimport) 
#endif 

extern "C" __DLL_EXP int c_abs(int value);
extern "C" __DLL_EXP long c_labs(long value);
extern "C" __DLL_EXP float c_fabs(float value);
extern "C" __DLL_EXP double c_dabs(double value);

extern "C" __DLL_EXP double c_round(double value,int figure);
extern "C" __DLL_EXP double c_ceil(double value);
extern "C" __DLL_EXP double c_floor(double value);
extern "C" __DLL_EXP double c_sqrt(double value);
extern "C" __DLL_EXP double c_pow(double value,float p);
extern "C" __DLL_EXP double c_percentage(double value);
extern "C" __DLL_EXP double c_random(int start,int end,int figure);