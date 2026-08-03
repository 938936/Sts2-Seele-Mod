"""
读取 docs/设计文档.xlsx，获取sheet页列表和sheet页内容。

用法 (通过 opencode Bash 工具执行):
  python .opencode/scripts/read_design_doc.py                  # 列出所有sheet页名称
  python .opencode/scripts/read_design_doc.py 卡牌             # 输出"卡牌"sheet的全部内容(JSON)
  python .opencode/scripts/read_design_doc.py 卡牌 坍缩攻击     # 在"卡牌"sheet中搜索"坍缩攻击"所在行
  python .opencode/scripts/read_design_doc.py 遗物             # 输出"遗物"sheet的全部内容(JSON)
  python .opencode/scripts/read_design_doc.py 能力 量子叠加     # 在"能力"sheet中搜索"量子叠加"所在行
"""

import openpyxl
import json
import sys
import os

sys.stdout.reconfigure(encoding='utf-8')

XLSX_PATH = os.path.join(os.path.dirname(__file__), "..", "..", "docs", "设计文档.xlsx")


def load_workbook():
    return openpyxl.load_workbook(XLSX_PATH, data_only=True)


def list_sheets():
    wb = load_workbook()
    print(json.dumps(wb.sheetnames, ensure_ascii=False))


def read_sheet(sheet_name):
    wb = load_workbook()
    if sheet_name not in wb.sheetnames:
        print(json.dumps({"error": f"Sheet '{sheet_name}' not found. Available: {wb.sheetnames}"}, ensure_ascii=False))
        sys.exit(1)
    ws = wb[sheet_name]
    rows = []
    for row in ws.iter_rows(values_only=True):
        rows.append([str(c) if c is not None else "" for c in row])
    print(json.dumps(rows, ensure_ascii=False, indent=2))


def search_sheet(sheet_name, keyword):
    wb = load_workbook()
    if sheet_name not in wb.sheetnames:
        print(json.dumps({"error": f"Sheet '{sheet_name}' not found. Available: {wb.sheetnames}"}, ensure_ascii=False))
        sys.exit(1)
    ws = wb[sheet_name]
    header = None
    results = []
    for i, row in enumerate(ws.iter_rows(values_only=True)):
        cells = [str(c) if c is not None else "" for c in row]
        if i == 0:
            header = cells
            continue
        if any(keyword in c for c in cells):
            results.append(dict(zip(header, cells)))
    print(json.dumps(results, ensure_ascii=False, indent=2))


if __name__ == "__main__":
    if len(sys.argv) == 1:
        list_sheets()
    elif len(sys.argv) == 2:
        read_sheet(sys.argv[1])
    elif len(sys.argv) >= 3:
        search_sheet(sys.argv[1], sys.argv[2])
