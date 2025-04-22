import json, os, re


LANG_PATH = os.path.dirname(os.path.abspath(__file__)) + "/all_lang.json"
OUT_PATH = os.path.dirname(os.path.abspath(__file__)) + "/"

f = open(LANG_PATH, 'r', encoding='utf-8')
contents = f.read()
f.close()
json_lang = json.loads(contents)

text_file = {
    '1': "zh_CN.txt",
    '2': "en_US.txt",
}

lua_content = []
lua_content.append("-- This file is generated automatically. Do not change it manually!")
lua_content.append("")
lua_content.append("local M = {")

text_content1 = []
text_content2 = []

text_string_enums = []


index = 0

for xls_id in json_lang:
    if xls_id != "language":
        for k in json_lang[xls_id]:
            # print(k)
            pos = k.find(xls_id) + len(xls_id)
            id2 = k[pos:]
            for lang in json_lang[xls_id][k]:
                # print(lang)
                # pos = lang.find('Sid')
                # id1 = lang[:pos]
                # print(json_lang[xls_id][k][lang])
                kk = 'SID_' + xls_id.upper() + lang.upper() + id2
                lua_content.append("    " + kk + " = " + str(index) + ",")
                text_string_enums.append(kk)
                index = index + 1

                lang1 = json_lang[xls_id][k][lang]["1"]
                text_content1.append(lang1)

               
                # < 英语
                lang2 = json_lang[xls_id][k][lang]["2"]
                if len(lang2) == 0:
                    text_content2.append(lang1)
                else:
                    text_content2.append(lang2)
                # <
    else:
        for k in json_lang[xls_id]:
            for lang in json_lang[xls_id][k]:
                # print(json_lang[xls_id][k][lang])
                kk = k
                lua_content.append("    " + kk + " = " + str(index) + ",")
                text_string_enums.append(kk)
                index = index + 1
                lang1 = json_lang[xls_id][k][lang]["1"]
                text_content1.append(lang1)


               # < 英语
                lang2 = json_lang[xls_id][k][lang]["2"]
                if len(lang2) == 0:
                    text_content2.append(lang1)
                else:
                    text_content2.append(lang2)
                # <

lua_content.append("}")
lua_content.append("")
lua_content.append("declare(\"SID\", M)")

# f = open("../Assets/Lua/app/_automatical/StringEnums.lua", 'w+', encoding='utf-8')
# f.write('\n'.join(lua_content))
# f.close()


# f = open(OUT_PATH + text_file['1'], 'w+', encoding='utf-8')
# f.write('\n'.join(text_content1))
# f.close()

# f = open(OUT_PATH + text_file['2'], 'w+', encoding='utf-8')
# f.write('\n'.join(text_content2))
# f.close()

f = open(OUT_PATH + "/StringEnums.txt", 'w+', encoding='utf-8')
f.write('\n'.join(text_string_enums))
f.close()


def replace(file, langIndex):
    file_data = ""
    with open(file, "r", encoding="utf-8") as f:
        for line in f:
            regex = re.compile(r"\[\d+\]")
            results = re.findall(regex, line)
            if len(results) > 0:
                for i in range(len(results)):
                    id_str = results[i][1:-1]               
                    lang_str = '"' + json_lang["hero"]["hero_" + id_str]["_nameSid"][langIndex] + '"'
                    
                    print(line)
                    line = line.replace(results[i], lang_str)
                    print(line)
                    print("\n")

            file_data += line
    
    with open(file,"w",encoding="utf-8") as f:
        f.write(file_data)

print(f"Current working directory: {os.getcwd()}")
replace("Assets/Multi_Lang/zh_CN.txt", "1")
replace("Assets/Multi_Lang/en_US.txt", "2")