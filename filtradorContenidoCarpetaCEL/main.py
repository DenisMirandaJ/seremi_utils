import os
import shutil

from time import sleep

def readDirectory(dir_name):
    """[summary]

    Args:
        dir_name (string): [description]
    """    

    # obtenemos el Desktop 
    desktop = os.path.join(os.path.join(os.path.expanduser('~')), 'Desktop')
    
    finalFolder = os.path.join(desktop,"Resultados\Febrero")

    contador = 0
    content = os.listdir(dir_name)
    

    # recorrido de la carpeta raiz
    for file in content: # file 
        
        # carpeta con fechas del mes
        folder = str(file)
        
        # ruta de esa carpeta y su contenido 
        sub_content_folder = os.path.join(dir_name,folder)
        os.chmod(sub_content_folder,777)
        sub_content= os.listdir(sub_content_folder)
        
        directory_destiny = os.path.join(finalFolder,folder)
       
        os.makedirs(directory_destiny , exist_ok=True)

        print(sub_content_folder)
        
        # recorrido de cada caso 
        for patient_folder in sub_content: 

            name_patient_dir = str(patient_folder)
            
            dir_patient  = os.path.join(sub_content_folder,name_patient_dir)
            
            os.chmod(dir_patient,777)
            cpt = sum([len(files) for r, d, files in os.walk(dir_patient)])


            f, ext =  os.path.splitext(dir_patient)

            if cpt < 4 and os.path.isdir(dir_patient):
                
                final_path = os.path.join(directory_destiny,name_patient_dir)
                os.makedirs(final_path, exist_ok=True)
                patient_data = os.listdir(dir_patient)
                
                for data in patient_data:
                    str_data = str(data)
                    
                    path_data = os.path.join(dir_patient,str_data) 
                    destination = shutil.copy2(path_data,final_path)
                contador = contador + 1;                

        print("")
        print("--------------------------------")
        print("")
    print("Contador: ",contador)

print("Ingrese ruta de la carpeta")
dir = input()

readDirectory(dir)