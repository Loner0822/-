//---------------------------------------------------------------------------

#ifndef PenUnitH
#define PenUnitH
//---------------------------------------------------------------------------

class Pen {
public:
    int id, type, partner, company;
    long x, y;
    Pen() {
        id = type = 0;
        company = partner = -1;
        x = y = 0;
    }
    Pen(const Pen & rhs) {
        id = rhs.id, type = rhs.type, partner = rhs.partner, company = rhs.company;
        x = rhs.x, y = rhs.y;
    }
    ~Pen() {}
    Pen& operator = (const Pen & rhs) {
        id = rhs.id, type = rhs.type, partner = rhs.partner, company = rhs.company;
        x = rhs.x, y = rhs.y;
        return *this;
    }

    bool operator <(const Pen &rhs) const
    {
        if (id != rhs.id)
            return id < rhs.id;
        else if (type != rhs.type)
            return type < rhs.type;
        return 0;
    }
};

#endif
