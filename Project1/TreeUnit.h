//---------------------------------------------------------------------------
#include <vcl.h>
#ifndef TreeUnitH
#define TreeUnitH
//---------------------------------------------------------------------------

template <class T>
class TNode {
public:
    T Data;
    int Head;
    TNode() {}
    ~TNode() {}
};

class TEdge {
public:
    int next;
    int to;
    TEdge() {}
    ~TEdge() {}
};


#endif
