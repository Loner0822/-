// metaex.h - Header for CMetaFileEx
#ifndef __CMETAFILE_H__
#define __CMETAFILE_H__

#include <stdio.h>
#include <windows.h>
// Placeable metafile data definitions
typedef struct tagOLDRECT
{
    short left;
    short top;
    short right;
    short bottom;
} OLDRECT;

#ifndef WORD
    #define WORD unsigned short
#endif
#ifndef DWORD
    #define DWORD unsigned long
#endif
// Placeable metafile header
typedef struct {
        DWORD   key;
        WORD	hmf;
        OLDRECT	bbox;
        WORD    inch;
        DWORD   reserved;
        WORD    checksum;
} ALDUSMFHEADER;

#define	METAFILE_VERSION	1
#define	ALDUSKEY		0x9AC6CDD7
#define	ALDUSMFHEADERSIZE	22	// Avoid sizeof is struct alignment >1

// Alignment types
typedef enum {
	AlignNone = -1,
	AlignDefault,
	AlignTopLeft,
	AlignTopCentre,
	AlignTopRight,
	AlignMiddleLeft,
	AlignMiddleCentre,
	AlignMiddleRight,
	AlignBottomLeft,
	AlignBottomCentre,
	AlignBottomRight,
	AlignStretch,
	AlignFit,
} METAALIGNMENT;

class CMetaFile
{
public:
	static bool IsMetafile(FILE* pFile);
	CMetaFile();
	~CMetaFile();
	void Display(HDC dc, RECT& r, METAALIGNMENT align = AlignDefault);
    bool PlayEnhMetaFile( HDC dc, RECT& r );
	bool Read(FILE* file);
	bool ReadFile(LPCTSTR lpszFileName);
	bool Read(char* buffer, int length = -1);
	void Clear();
	bool IsEmpty();
	// new:
	int GetWidth() const;
	int GetHeight() const;
	bool IsOK() const{return m_emf==NULL ? false : true;};
    HENHMETAFILE GetHandle(){return m_emf;}
    float GetInch(){ return m_inch; }
private:
	void FitPicture(RECT& r, SIZE& size, METAALIGNMENT align);
	ALDUSMFHEADER m_aldusMFHeader;
    float m_inch;
	HENHMETAFILE m_emf;
	HMETAFILE m_wmf;
};

#endif	// __CMETAFILE_H__
