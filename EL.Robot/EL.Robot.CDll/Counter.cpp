//
// Created by GYP on 2022/11/7.
//
#include "pch.h"
#include "Counter.h"
#include <stdlib.h>



Counter::Counter() {
    this->v1 = new vector<string>();
}

double Counter::coutn(const string &s) {
    this->v1->clear();//清空列表
    //string a = s;
    resolve(s);//分解输入的字符
    return transition(v1);
}


void Counter::resolve(const string &s) {
    int move = 0;
    //int cont = 0;
    //cout << s.length()<<endl;
    this->cNum = s.length() / 2;
    this->nNum = (s.length() / 2) + 1;
    for (int i = 0; i < s.size(); i++) {
        if (s[i] == '+' || s[i] == '-' || s[i] == '*' || s[i] == '/') {
            v1->push_back(s.substr(move, i - move));
            //cout << s.substr(move, i-move) << endl;
            move = i;
            v1->push_back(s.substr(move, 1));
            //cout << s.substr(move, 1) << endl;
            move += 1;
        } else if (i == s.length() - 1) {
            v1->push_back(s.substr(move, i + 1 - move));
            //cout << s.substr(move, i - move) << endl;
        }
    }
}

double Counter::transition(vector<string> *v) {
    char *c = new char[cNum];//运算符栈
    vector<string> cv;//转换后的运算式列表
    int sc1 = 0;
    for (int i = 0; i < v->size(); i++) {
        if (v->at(i) == "+" || v->at(i) == "-" || v->at(i) == "*" || v->at(i) == "/") {
            char temp = *(v->at(i).c_str());
            stack_top(c, temp, sc1, &cv);

        } else//不是运算符的情况
        {
            cv.push_back(v->at(i));
        }

    }
    for (int i = sc1 - 1; i >= 0; i--) {
        string s(1, c[i]);
        cv.push_back(s);
    }


    delete[] c;
    return count(cv);
}

int Counter::c_sort(const char &c) {
    switch (c) {
        case '+':
            return 0;
        case '-':
            return 0;
        case '*':
            return 1;
        case '/':
            return 1;
        default:
            return 0;
    }
}

void Counter::stack_top(char *c, char temp, int &index, vector<string> *_v) {
    if (index > 0) {
        if (c_sort(temp) <= c_sort(c[(index - 1)])) {
            string s(1, c[(index - 1)]);
            _v->push_back(s);
            index--;
            stack_top(c, temp, index, _v);
            return;
        } else {
            c[index] = temp;
            index++;
            return;
        }
    } else if (index == 0) {
        c[index] = temp;
        index++;
        return;
    }

}

double Counter::count(vector<string> &cv) {
    double *p_d = new double[nNum];
    int d_index(0);
    double d;
    double n1, n2;
    for (int i = 0; i < cv.size(); i++) {
        auto a = cv.at(i).c_str();
        switch (*a) {
            case '+':
                n1 = p_d[--d_index];
                n2 = p_d[--d_index];
                p_d[d_index++] = n1 + n2;
                break;
            case '-':
                n1 = p_d[--d_index];
                n2 = p_d[--d_index];
                p_d[d_index++] = n2 - n1;
                break;
            case '*':
                n1 = p_d[--d_index];
                n2 = p_d[--d_index];
                p_d[d_index++] = n1 * n2;
                break;
            case '/':
                n1 = p_d[--d_index];
                n2 = p_d[--d_index];
                p_d[d_index++] = n2 / n1;
                break;
            default:
                d = atof(a);
                p_d[d_index++] = d;
        }


    }

    double over = p_d[0];
    delete[]p_d;
    return over;
}

Counter::~Counter() {
    delete v1;
}