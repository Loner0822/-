//---------------------------------------------------------------------------

#ifndef TDataStructH
#define TDataStructH
#include <vcl.h>
#include <map.h>
#include <iniFiles.hpp>
#include <sstream>
#include <memory>
#include <ComObj.hpp>
//---------------------------------------------------------------------------
//��������
typedef struct Param
{
    AnsiString name;
    AnsiString type;
    AnsiString s_value;  //��ʾ��
    char *c_value; //ԭʼ����
    int size;
}TParam;

typedef map<AnsiString, TParam> TParamMap;
typedef map<AnsiString, TParam>::iterator TitParamMap;

//�ṹ��
class TParamGroup : public TObject
{
private:
    map<AnsiString, TParam> ParamGroup;         //����
    TObjectList *ParamGroupList;  //�ṹ���а��������ṹ����������
private:
    AnsiString flag;
    AnsiString flagEx;
    TStringList *iniParamList;
    void ReadIniFile();
public:
    TParamGroup( AnsiString flag, AnsiString flagEx = "" );      //���������ļ��ڵ�
    __fastcall ~TParamGroup(){};
    void Init();
    int getParamCount();
    TParam* getParam( const AnsiString &key );   //����0
    TParam* getParamByName( const AnsiString &name );
    AnsiString getFlag(){ return flag; };
    AnsiString getValueByName( AnsiString name );
    char* getCValueByName( AnsiString name );

    void AddSubStruct( TParamGroup *pg );
    int getSubStructCount();
    TParamGroup *getSubStruct( int i );   //0

    int Translate( char *& org, int begin, int length, TParamGroup *groupEx = NULL, TParamGroup *groupEx1 = NULL );

    bool IsDefaultValue( TParam *pm, TParamGroup *groupEx = NULL,TParamGroup *groupEx1 = NULL );

};
#endif
