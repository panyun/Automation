# -*- coding: utf-8 -*-
# 第一行的目的，是为了让代码里面，可以有中文信息. (will否则要运行报错)
# 这个 Python 脚本， 用于被 C# 来调用.

def welcome(name):
    print ("hello （from Python） " + name)
    return "hello " + name

# 测试 参数为 C# 对象的效果. (获取/设置 C# 对象的属性)
def testAddAge(obj):
    obj.Age = obj.Age + 1
    obj.Desc = obj.UserName + " 又大了一岁，测试是否中文乱码（from Python）"
    print(obj.Desc)

# 测试 参数为 C# 对象的效果. (调用 C# 对象的方法)
def testAddAge2(obj):
    obj.AddAge(2)

# 测试 List.
def testList(lst):
    vResult = ""
    for each_item in lst:
        vResult = vResult + " " + each_item
    return vResult
# 测试 Set.
def testSet(pSet):
    vResult = ""
    for each_item in pSet:
        vResult = vResult + " " + each_item
    return vResult

# 测试 Dictionary
def testDictionary(pDictionary):
    vResult = ""
    for each_item in pDictionary:
        vResult = vResult + " " + each_item + "=" + pDictionary[each_item] + ";"
    return vResult
