import pandas as pd
import os


def rut_notificaciones_a_rut_esmeralda(identificacion: str, dv: str) -> str:
    if dv == '-':
        return identificacion
    return identificacion + '-' + dv

def obtener_trazadores(turno_file_path: str) -> list[str]:
    trazadores: pd.DataFrame = pd.read_excel(turno_file_path, engine="openpyxl")
    trazadores_presentes = trazadores[~trazadores['Presente?'].isin(['No', 'NO', 'no', 'nO'])]
    return trazadores_presentes['Trazador'].to_list()

def check_casos_prioritarios(row: pd.Series) -> list[str]:
    categoria_prioritarios = []
    lista_comorbilidades = ["Diabetes","Asma","Enfermedad Pulmonar Crónica","Hipertensión Arterial","Cardiopatía Crónica","Enfermedad Neurológica Crónica","Enfermedad Renal Crónica","Inmunocomprometido (Enfermedad O Tratamiento)","Enfermedad Hepática Crónica","Obesidad",]
    if row['embarazo'] == 't':
        categoria_prioritarios.append("Embarazada")
    if int(row['edad']) > 60: 
        categoria_prioritarios.append("Adulto mayor")
    if not pd.isna(row['comorbilidad']):
        tiene_comorbilidad = [(comorbilidad in row['comorbilidad']) for comorbilidad in lista_comorbilidades]
        if True in tiene_comorbilidad:
            categoria_prioritarios.append("Comorbilidad")
    if len(categoria_prioritarios) == 0:
        return None
    return ", ".join(categoria_prioritarios)

def append_df_to_excel(filename, df, sheet_name='Sheet1', startrow=None,
                       truncate_sheet=False, 
                       **to_excel_kwargs):
    """
    Append a DataFrame [df] to existing Excel file [filename]
    into [sheet_name] Sheet.
    If [filename] doesn't exist, then this function will create it.

    @param filename: File path or existing ExcelWriter
                     (Example: '/path/to/file.xlsx')
    @param df: DataFrame to save to workbook
    @param sheet_name: Name of sheet which will contain DataFrame.
                       (default: 'Sheet1')
    @param startrow: upper left cell row to dump data frame.
                     Per default (startrow=None) calculate the last row
                     in the existing DF and write to the next row...
    @param truncate_sheet: truncate (remove and recreate) [sheet_name]
                           before writing DataFrame to Excel file
    @param to_excel_kwargs: arguments which will be passed to `DataFrame.to_excel()`
                            [can be a dictionary]
    @return: None

    Usage examples:

    >>> append_df_to_excel('d:/temp/test.xlsx', df)

    >>> append_df_to_excel('d:/temp/test.xlsx', df, header=None, index=False)

    >>> append_df_to_excel('d:/temp/test.xlsx', df, sheet_name='Sheet2',
                           index=False)

    >>> append_df_to_excel('d:/temp/test.xlsx', df, sheet_name='Sheet2', 
                           index=False, startrow=25)

    (c) [MaxU](https://stackoverflow.com/users/5741205/maxu?tab=profile)
    """
    # Excel file doesn't exist - saving and exiting
    if not os.path.isfile(filename):
        df.to_excel(
            filename,
            sheet_name=sheet_name, 
            startrow=startrow if startrow is not None else 0, 
            **to_excel_kwargs)
        return
    
    # ignore [engine] parameter if it was passed
    if 'engine' in to_excel_kwargs:
        to_excel_kwargs.pop('engine')

    writer = pd.ExcelWriter(filename, engine='openpyxl', mode='a', datetime_format='DD-MM-YYYY')

    # try to open an existing workbook
    writer.book = load_workbook(filename)
    
    # get the last row in the existing Excel sheet
    # if it was not specified explicitly
    if startrow is None and sheet_name in writer.book.sheetnames:
        startrow = writer.book[sheet_name].max_row

    # truncate sheet
    if truncate_sheet and sheet_name in writer.book.sheetnames:
        # index of [sheet_name] sheet
        idx = writer.book.sheetnames.index(sheet_name)
        # remove [sheet_name]
        writer.book.remove(writer.book.worksheets[idx])
        # create an empty sheet [sheet_name] using old index
        writer.book.create_sheet(sheet_name, idx)
    
    # copy existing sheets
    writer.sheets = {ws.title:ws for ws in writer.book.worksheets}

    if startrow is None:
        startrow = 0

    # write out the new sheet
    df.to_excel(writer, sheet_name, startrow=startrow, **to_excel_kwargs)

    # save the workbook
    writer.save()