object DMod: TDMod
  OldCreateOrder = False
  OnCreate = DataModuleCreate
  Left = 520
  Top = 262
  Height = 150
  Width = 216
  object ADOConnection1: TADOConnection
    ConnectionString = 
      'Provider=Microsoft.Jet.OLEDB.4.0;User ID=Admin;Data Source=D:\Co' +
      'de\WorkingSpace\Project1\data\YjzhZsk.mdb;Mode=Share Deny None;P' +
      'ersist Security Info=False;Jet OLEDB:System database="";Jet OLED' +
      'B:Registry Path="";Jet OLEDB:Database Password="";Jet OLEDB:Engi' +
      'ne Type=5;Jet OLEDB:Database Locking Mode=1;Jet OLEDB:Global Par' +
      'tial Bulk Ops=2;Jet OLEDB:Global Bulk Transactions=1;Jet OLEDB:N' +
      'ew Database Password="";Jet OLEDB:Create System Database=False;J' +
      'et OLEDB:Encrypt Database=False;Jet OLEDB:Don'#39't Copy Locale on C' +
      'ompact=False;Jet OLEDB:Compact Without Replica Repair=False;Jet ' +
      'OLEDB:SFP=False'
    Provider = 'Microsoft.Jet.OLEDB.4.0'
    Left = 32
  end
  object ADOConnection2: TADOConnection
    ConnectionString = 
      'Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\Code\WorkingSpac' +
      'e\Project1\data\'#24212#24613#25351#25381#32467#32447#22270'.mdb;Persist Security Info=False'
    Mode = cmShareDenyNone
    Provider = 'Microsoft.Jet.OLEDB.4.0'
    Left = 32
    Top = 48
  end
  object ADOConnection3: TADOConnection
    ConnectionString = 
      'Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\Code\WorkingSpac' +
      'e\Project1\data\'#32467#28857#23646#24615'.mdb;Persist Security Info=False'
    Provider = 'Microsoft.Jet.OLEDB.4.0'
    Left = 120
  end
  object ADOConnection4: TADOConnection
    ConnectionString = 
      'Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\Code\WorkingSpac' +
      'e\Project1\data\ZSK_H0000Z000K06.mdb;Persist Security Info=False'
    Provider = 'Microsoft.Jet.OLEDB.4.0'
    Left = 120
    Top = 48
  end
end
