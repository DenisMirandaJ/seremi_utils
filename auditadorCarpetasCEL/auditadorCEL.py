import os
import shutil

def auditarCEL(planillas_dirname, respaldo_paciente_dirname):
  files_in_planillas = []
  for root, dirs, files in os.walk(planillas_dirname, topdown=False):
    for file in files:
      files_in_planillas.append(str(file).encode('utf8'))

  # not_in_planillas = []
  # for root, dirs, files in os.walk(respaldo_paciente_dirname, topdown=False):
  #   if not os.path.splitext(files) not in ('xlsx', 'xls'):
  #     os.remove()
  #   if files not in files_in_planillas:
  #     not_in_planillas.append([files, dirs])

  open("a.txt", "w").writelines(files_in_planillas)

auditarCEL('C:/seremi/bases/FEBRERO', "")