#include "pch.h"
#include "MathFun.h"
#include<cmath>

double c_round(double value,int figure)
{
	if(figure == 0){
		return round(value);
	}
	double f = pow(10,figure+1);
	double a = f/10;
	double temp = (value+(5/f))*a;
	double result = round(temp)/a;
	return result;
}
double c_ceil(double value)
{
	return ceil(value);
}
double c_floor(double value)
{
	return floor(value);
}

double c_pow(double value,float p)
{
	if(value<0){
		return -1;
	}
	return pow(value,p);
}
double c_sqrt(double value)
{
	if(value<0){
		return -1;
	}
	return sqrt(value);

}
double c_percentage(double value)
{
	return value*100.0;
}

double c_random(int start,int end,int figure)
{
	if(figure<0){
		return rand()%end*1.0+start;
	}
	if(figure == 0){
		return 1.0*(rand() % (end - start + 1) + start);
	}

	double p = pow(10,figure);
	return (rand()%(int)(end*p - start*p + 1) + start*p) / p;
}

int c_abs(int value)
{
	return abs(value);
}
long c_labs(long value)
{
	return abs(value);
}
float c_fabs(float value)
{
	return abs(value);
}
double c_dabs(double value)
{
	return abs(value);
}