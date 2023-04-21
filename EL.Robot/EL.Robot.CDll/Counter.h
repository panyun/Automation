//
// Created by GYP on 2022/11/7.
//

#ifndef ROBOT_COUNTER_H
#define ROBOT_COUNTER_H

 
#include <string>
#include <vector>
using std::string;
using std::vector;
class Counter
{
public:
    Counter();
    double coutn(const string &s);
    ~Counter();
private:
    void resolve(const string &s);//分解字符串的函数
    double transition(vector<string> *v);//正常算式转换成后缀算式
    int c_sort(const char &c);//计算运算符的优先级
    void stack_top(char *c,char temp, int &index, vector<string> *v);//运算符压栈
    double count(vector<string> &v);//把后缀式计算得到结果
    vector<string> *v1;
    int nNum;
    int cNum;
};

#endif //ROBOT_COUNTER_H
