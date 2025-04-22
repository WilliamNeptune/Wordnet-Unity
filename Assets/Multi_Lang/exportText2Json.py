import os
import xlrd
import json
import copy

LANG_NUM = 7

LANG_PATH = os.path.dirname(os.path.abspath(__file__)) + "/../Resources/"

if os.path.exists(LANG_PATH + "all_lang.json") == False:
    f = open(LANG_PATH + "all_lang.json", 'w+', encoding='utf-8')
    f.write('{')
    f.write('"language": {')
    f.write('}')
    f.write('}')
    f.close()    

f = open(LANG_PATH + "all_lang.json", 'r', encoding='utf-8')
contents = f.read()
if contents.startswith(u'\ufeff'):
	contents = contents.encode('utf8')[3:].decode('utf8')
f.close()
json_lang = json.loads(contents)


def walk(list_files, filepath):
    files = os.listdir(filepath)
    for file_str in files:
        fi_d = os.path.join(filepath, file_str)            
        if os.path.isfile(fi_d) and ('.xls' in file_str):
            list_files.append(LANG_PATH + "../" + file_str)

#list_files = []
# walk(list_files, LANG_PATH + '../')

def sort_dict_wity_key(fileKey, keys):
    copyDict = copy.deepcopy(json_lang[fileKey])
    json_lang[fileKey] = {}
    
    for key in keys:
        json_lang[fileKey][key] = copyDict[key]

def sheet_cell(sheet, row, col):
    val = sheet.cell(row, col).value
    try:
        val = str(int(val))
    except:
        val = str(val)
            
    return val

def export_lang(fileXls):
    global fileKey

    wordbook = xlrd.open_workbook(fileXls)
    sheetMain = wordbook.sheet_by_index(0)

    sheetNames = wordbook.sheet_names()

    sheetName = {}
    sheetDesc = {}
    
    rowKeys = []
    rowTypes = []

    id_col_index = 0
    for col in range(sheetMain.ncols):
        value = sheet_cell(sheetMain, 2, col)
        if (value == '_id'):
            id_col_index = col
        rowKeys.append(value)
        rowTypes.append(sheet_cell(sheetMain, 3, col))

    xlsx_lang = {}
    sub_keys = set()
    for col in range(0, len(rowTypes)):
        if (rowTypes[col] == 'D'):
            for row in range(4, sheetMain.nrows):
                _REF_Val_1 = 0
                _REF_Val = 0
                try:
                    val = sheetMain.cell(row, col).value
                    _REF_Val_1 = int(val)
                except:
                    _REF_Val_1 = 0

                if _REF_Val_1 == 0:
                    _REF_Val = 0
                    if rowTypes[col - 1] == 'R':
                        _REF_Val = sheetMain.cell(row, col - 1).value
                        try:
                            if _REF_Val[0] == "[" and _REF_Val[-1] == "]":
                                _REF_Val = int(_REF_Val[1:-1])
                        except:
                            _REF_Val = 0

                
                if (fileKey in json_lang.keys()) == False:
                    json_lang[fileKey] = {}

                subKey = fileKey + "_"+ str(int(sheetMain.cell(row, id_col_index).value))


                if (subKey in json_lang[fileKey].keys()) == False:
                    json_lang[fileKey][subKey] = {}

                xlsx_lang[subKey] = {}

                subKey1 = rowKeys[col]
                # print('subKey1==========',sheetMain.cell(row, id_col_index).value,  subKey, subKey1, _REF_Val, _REF_Val_1)
                sub_keys.add(subKey1)
                if _REF_Val > 0 or _REF_Val_1 > 0:
                    if (subKey1 in json_lang[fileKey][subKey].keys()) == True:
                        del json_lang[fileKey][subKey][subKey1]
                else:
                    if (subKey1 in json_lang[fileKey][subKey].keys()) == False:
                        json_lang[fileKey][subKey][subKey1] = {}

                    v = sheet_cell(sheetMain, row, col)
                    if v == "0":
                        v = ""

                    json_lang[fileKey][subKey][subKey1]['1'] = v
                    for i in range(2, LANG_NUM + 1):
                        if (str(i) in json_lang[fileKey][subKey][rowKeys[col]].keys()) == False:
                            json_lang[fileKey][subKey][subKey1][str(i)] = ''
                    


    new_keys = []
    for row in range(4, sheetMain.nrows):
        subKey = fileKey + "_"+ str(int(sheetMain.cell(row, id_col_index).value))
        new_keys.append(subKey)

    # print(sub_keys)

   


    # print('sub_key_set=========== ', sub_key_set)
    # 删除多于的行
    
    old_keys = json_lang[fileKey].keys()
    dif_keys = list(set(old_keys).difference(set(new_keys)))
    
    list_old_keys = list(old_keys)
    for i in range(0, len(list_old_keys)):
        old_key = list_old_keys[i]
        list_old_sub_keys = list(json_lang[fileKey][old_key].keys())
        for j in range(0, len(list_old_sub_keys)):
            sub_old_key = list_old_sub_keys[j]
            if (sub_old_key in sub_keys) == False:
                del json_lang[fileKey][old_key][sub_old_key]

    # print("dif_keys -====== ", dif_keys)
    for i in range(0, len(dif_keys)):
        dif_key = dif_keys[i]
        del json_lang[fileKey][dif_key]



    # 调整顺序
    sort_dict_wity_key(fileKey, new_keys)
    
def read_file(fliepath):
    lines = []
    f = open(fliepath, 'r', encoding = 'utf-8')
    for line in f:
        lines.append(line.strip('\n'))
    f.close()

    # os.remove(fliepath) 

    return lines

#file_keys = read_file(LANG_PATH + 'lang_xlsx.txt')



# global fileKey
# for file in file_keys:
#     i = file.find('.xls')
#     fileKey = file[0:i]
#     # if file != 'chasm.xlsx':
#     #     continue
#     export_lang(LANG_PATH + "../" + file)

sorted_json_lang = {}
# for file in file_keys:
#     i = file.find('.xls')
#     file_key = file[0:i]
#     sorted_json_lang[file_key] = json_lang[file_key]




def read_from_language_cvs():
    # Update path to use LANG_PATH which is already correctly defined
    csv_path = LANG_PATH + 'text.csv'
    
    old_keys = json_lang['language'].keys()
    new_keys = []
    add_keys = []

    f_a = open(csv_path, 'r', encoding='utf-8')
    lineStr = f_a.readline()
    while lineStr:
        a = lineStr.find(',')
        if a > 0:
            key = lineStr[:a]
            content = lineStr[a+1:].strip('\n')
            # content = content.replace("\n", "\\n")
            if (key in json_lang['language'].keys()) == False:
                add_keys.append(key)
                json_lang['language'][key] = {
                    '_descSid': {
                        '1': '',
                        '2': '',
                        '3': '',
                        '4': '',
                        '5': '',
                        '6': '',
                        '7': '',
                    }
                }

            json_lang['language'][key]['_descSid']['1'] = content
            new_keys.append(key)

        lineStr = f_a.readline()

    dif_keys = list(set(old_keys).difference(set(new_keys)))
    for i in range(0, len(dif_keys)):
        dif_key = dif_keys[i]
        del json_lang['language'][dif_key]
    
    print('language add keys: ', add_keys)
    print('language remove keys: ', dif_keys)

    f_a.close()

    # 调整顺序
    sort_dict_wity_key('language', new_keys)
    
read_from_language_cvs()  

sorted_json_lang['language'] = json_lang['language']


def remove_empty_value():
    remove_keys = []
    remove_row_keys = []
    for xls_key in sorted_json_lang:
 
        # if xls_key != "iap_task":
        #     continue
        for row_key in sorted_json_lang[xls_key]:
            # print(row_key, sorted_json_lang[xls_key][row_key])

            if not bool(sorted_json_lang[xls_key][row_key]):  
                remove_row_keys.append(row_key)
            else:
                for type_key in sorted_json_lang[xls_key][row_key]:
                    if len(sorted_json_lang[xls_key][row_key][type_key]['1']) == 0:
                        remove_keys.append(row_key + type_key)

        sorted_json_lang[xls_key]
        for row_key in remove_row_keys:
            # print(row_key)
            if row_key in sorted_json_lang[xls_key].keys():
                del sorted_json_lang[xls_key][row_key]
        

    for key in remove_keys:
        pos1 = key.rfind('_')
        type_key = key[pos1:]
        row_key = key[:pos1]
        pos2 = row_key.rfind('_')
        xls_key = row_key[:pos2]
        print(type_key, row_key, xls_key)
        del sorted_json_lang[xls_key][row_key][type_key]

        if not sorted_json_lang[xls_key][row_key]:
            del sorted_json_lang[xls_key][row_key]


remove_empty_value()

out_str = json.dumps(sorted_json_lang, sort_keys = False, indent = 4, ensure_ascii = False)
f = open(LANG_PATH + 'all_lang.json', 'w+', encoding='utf-8')
f.write(out_str)
f.close()

if __name__ == "__main__":
    # Update paths to use LANG_PATH
    csv_file_path = LANG_PATH + "text.csv"
    json_file_path = LANG_PATH + "text.json"
    convert_csv_to_json(csv_file_path, json_file_path)

