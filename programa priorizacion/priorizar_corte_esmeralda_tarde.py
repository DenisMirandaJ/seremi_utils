import pandas as pd
import casos_dia_utils
import random
import os
from datetime import datetime
import time

def limpiar_base_notificaciones_positivos(notificaciones_path: str) -> pd.DataFrame:
    """Limpia la base de notificaciones para dejar solo casos no descar]tados o eliminados

    Args:
        notificaciones_path (str): path de la base de notificaciones mas reciente

    Returns:
        pd.DataFrame: DataFrame con los casos no descartados o eliminados
    """
    notificaciones_df: pd.DataFrame = pd.read_csv(notificaciones_path, sep=";")
    cond1 = notificaciones_df['vigente_no_eliminado'] == 't'
    cond2 = notificaciones_df['estado_caso'] != 'No validada'
    cond3 = (notificaciones_df['etapa_clinica'] == "CONFIRMADA") | (notificaciones_df['etapa_clinica'] == "PROBABLE") | (notificaciones_df['etapa_clinica'] == "SOSPECHA")
    notificaciones_df = notificaciones_df[cond1 & cond2 & cond3]
    
    notificaciones_df['fecha_notificacion'] = pd.to_datetime(notificaciones_df['fecha_notificacion'], format='%Y-%m-%d', errors='coerce')
    notificaciones_df['rut_estilo_esmeralda'] = notificaciones_df.apply(
        lambda row: casos_dia_utils.rut_notificaciones_a_rut_esmeralda(row['identificacion_paciente'], row['dv']), axis=1
    )
    notificaciones_df = notificaciones_df.sort_values(by=['fecha_notificacion'])
    return notificaciones_df

def categorizar_trazadores(casos_priorizados_anterior_path: str)->pd.DataFrame:
    
    # Lectura de archivo primer corte 
    casos_priorizados: pd.DataFrame = pd.read_excel(casos_priorizados_anterior_path)
    
    # Rescate de las columnas necesarias del df  
    trazadores_df = casos_priorizados[['trazador','numero_de_contactos']]

    # Sumamos 1 ya que se considera el llamado al caso indice
    trazadores_df['numero_de_contactos']+=1
    
    # Sumamos las cantidades de llamadas y los ordenamos por trazador
    trazadores_df=trazadores_df.groupby('trazador').sum().sort_values(by=['numero_de_contactos'])
    
    return trazadores_df['trazador']



def priorizar_corte_examenes(lista_examenes_path: str, notificaciones_path: str, lista_trazadores_path: str, casos_priorizados_anterior_path: str) -> pd.DataFrame:
    """Prioriza y asigna los casos de un corte 

    Args:
        lista_examenes_path (str): Excel con examenes positivos con formato esmeralda
        notificaciones_path (str): path de la base de notificaciones mas reciente
        lista_trazadores_path (str): Lista trazadores

    Returns:
        pd"""
    # formateo de examenes a desde esmeralda
    examenes_df: pd.DataFrame = pd.read_excel(lista_examenes_path)
    examenes_df['fecha_muestra'] = pd.to_datetime(examenes_df['fecha_muestra'], format='%Y-%m-%d', errors='coerce')
    
    notificaciones_df: pd.DataFrame = limpiar_base_notificaciones_positivos(notificaciones_path)
    notificaciones_df = notificaciones_df.drop_duplicates(subset=['rut_estilo_esmeralda'])
    # notificaciones_df = notificaciones_df.

    join_examenes = examenes_df.set_index(['run']).join(notificaciones_df.set_index('rut_estilo_esmeralda'), rsuffix='_not').reset_index()
    join_examenes = join_examenes.rename({'index': 'run'}, axis=1)
    join_examenes['prioridad'] = join_examenes.apply(
        lambda row: casos_dia_utils.check_casos_prioritarios(row), axis=1
    )
    join_examenes = join_examenes.sort_values(by=['prioridad', 'fecha_muestra'])

    trazadores = casos_dia_utils.obtener_trazadores(lista_trazadores_path)
    


    casos_trazadores_dic = {trazador: [] for trazador in trazadores}
    for i, row in join_examenes.iterrows():
        trazador = trazadores[i%len(trazadores)]
        casos_trazadores_dic[trazador].append(row)

    date_str = datetime.now().strftime('%d-%m-%Y %H_%M')
    join_examenes.insert(join_examenes.columns.get_loc('folio_monitor')+1,'trazador','')
    join_examenes['trazador'] = [trazadores[i%len(trazadores)] for i in range(len(join_examenes))]
    join_examenes['folio_epivigila'] = ''
    join_examenes['numero_de_contactos'] = ''
    cols_to_keep = [
        'run', 
        'nombre',
        'edad',
        'comuna',
        'trazador', 
        'prioridad',
        'pais',
        'fecha_muestra',
        'folio_epivigila',
        'numero_de_contactos'
    ]
    

    join_examenes['fecha_muestra'] = join_examenes['fecha_muestra'].dt.strftime('%d-%m-%Y')
    join_examenes = join_examenes[cols_to_keep].sort_values(by=['trazador', 'fecha_muestra'])
    return join_examenes.to_excel('esmeralda_priorizado_{}.xlsx'.format(date_str), engine="openpyxl", index=False)

    
if __name__ == '__main__':
    t = time.time()
    """priorizar_corte_examenes(
        'esmeralda-28-03-2021-1.xlsx',
        '20210328_Región de Antofagasta_notificaciones_0600.csv ',
        'trazadores turno 1.xlsx',
        'esmeralda_priorizado_29-03-2021 11_04.xlsx'

    )"""
    categorizar_trazadores('esmeralda_priorizado_29-03-2021 11_04.xlsx')

    print(time.time() - t)