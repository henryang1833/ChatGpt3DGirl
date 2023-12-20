#include <iostream>

class Base {
public:
    void show() {
        std::cout << "Base show" << std::endl;
    }
};

class Derived : public Base {
public:
    void show() {
        std::cout << "Derived show" << std::endl;
    }
};

int main() {
    Base* b = new Derived();
    b->show(); // 调用 Base 的 show，输出 "Base show"

    Derived* d = new Derived();
    d->show(); // 调用 Derived 的 show，输出 "Derived show"

    delete b;
    delete d;
    return 0;
}
