#ifndef DRAWINGINFOHPP
#define DRAWINGINFOHPP

#pragma delphiheader begin
#pragma option push -w-
#pragma option push -Vx
#include <Classes.hpp>
#include <extctrls.hpp>

#include <vector>

class TShapeInfo
{
public:
    TShapeInfo();
public:
    int GetId(){ return m_id; };
    int GetGroup(){ return m_group; };
    int GetType(){ return m_type; };
    int GetOriginType(){ return m_orgType; };
    int GetIconId(){ return m_iconId; };
    AnsiString& GetGuid(){ return m_guid; };
    int HitTest( int x, int y );
    bool LoadFromStream( TStream* pStream );
    bool LoadFromStream2( TStream* pStream );
    std::vector < TRect >* GetBoundingBoxes(){ return &m_graphBBoxes; }
    TRect GetTextBox(){ return m_textBBox; }
    TRect GetBoundingBox(){ return m_box; }
private:
    void UpdateBBox();
private:
    int m_id;
    int m_group;
    int m_type;
    int m_orgType;
    float m_angle;
    int x;
    int y;
    int width;
    int height;
    TRect m_textBBox;
    std::vector < TRect > m_graphBBoxes;
    TRect m_box;
    int m_iconId;
    AnsiString m_guid;
};

class TDrawingParam
{
public:
    int lineId;
    AnsiString lineName;
    int stationId;
    AnsiString stationName;
    int drawingId;
    AnsiString drawingName;
    int drawingType;
    int pageId;
    TDrawingParam(){
        lineId = stationId = drawingId = drawingType = pageId = 0;
    }
};

class TSigDrawing
{
public:
    TSigDrawing();
public:
    static Graphics::TBitmap* m_defBitmap16;
    enum emGraphType
    {
        gtBMP,
        gtEMF,
        gtWMF
    };
    TImage* GetImage(){ return m_image; }
    const TShapeInfo* HitTest( int x, int y );
    bool LoadFromStream( TStream* pStream );
    bool LoadFromCompressedStream( TStream* pStream );
    std::vector < TShapeInfo >* GetShapes(){ return &m_shapes; }
    bool SaveToBitmap(AnsiString bmpFile);
    bool SaveToGIF(AnsiString gifFile);

    __property int Type  = { read=GetType, write=SetType };
    __property emGraphType GraphType  = { read=GetGraphType, write=SetGraphType };

    __property TDrawingParam* Param  = { read=GetParam };

    AnsiString EnumShapes( std::vector<long>& idAll, long* Index );
private:
    int FDrawingID;
    TDrawingParam* GetParam(){ return &FParam; }
    TDrawingParam FParam;
    TImage* m_image;
    TRect m_box;
    std::vector < TShapeInfo > m_shapes;
    int FType;
    emGraphType FGraphType;
    void __fastcall SetType(int value);
    int __fastcall GetType();
    void __fastcall OnZipError(TStream* Stream, int & ErrorCode);
    void __fastcall SetGraphType(emGraphType value);
    emGraphType __fastcall GetGraphType();
    TShapeInfo* __fastcall GetShapes(int __index);
};


#endif