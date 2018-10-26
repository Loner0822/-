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
//基本参数
typedef struct Param
{
    AnsiString name;
    AnsiString type;
    AnsiString s_value;  //显示用
    char *c_value; //原始数据
    int size;
}TParam;

typedef map<AnsiString, TParam> TParamMap;
typedef map<AnsiString, TParam>::iterator TitParamMap;

//结构体
class TParamGroup : public TObject
{
private:
    map<AnsiString, TParam> ParamGroup;         //基本
    TObjectList *ParamGroupList;  //结构体中包含其他结构体类型数据
private:
    AnsiString flag;
    AnsiString flagEx;
    TStringList *iniParamList;
    void ReadIniFile();
public:
    TParamGroup( AnsiString flag, AnsiString flagEx = "" );      //根据配置文件节点
    __fastcall ~TParamGroup(){};
    void Init();
    int getParamCount();
    TParam* getParam( const AnsiString &key );   //基于0
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
