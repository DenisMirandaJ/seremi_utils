from __future__ import print_function
import pickle
import os.path
from googleapiclient.discovery import build
from google_auth_oauthlib.flow import InstalledAppFlow
from google.auth.transport.requests import Request
from openpyxl import load_workbook
from openpyxl.worksheet.table import Table
from openpyxl.utils import get_column_letter
from datetime import datetime
import pandas as pd


# If modifying these scopes, delete the file token.pickle.
SCOPES = ['https://www.googleapis.com/auth/spreadsheets.readonly']

# The ID and range of a sample spreadsheet.
SAMPLE_SPREADSHEET_ID = '1EgOCcij3h-jOuJm2HectmerkM0R6iFSFG3k1Ioh6J2k'
SAMPLE_RANGE_NAME = 'PLANILLA MADRE!A:AK'


def tracersNamesListed(tracer_list,tracer,data_dif,count_list):
    if not tracer in tracer_list:
        tracer_list.append(tracer)
        data_dif.append(0)
        count_list.append(1)
    
def subtractDates(date1,date2,typeData):
    try: 
        data_dt1= datetime.strptime(date1, '%d/%m/%Y')
        data_dt2= datetime.strptime(date2, '%d/%m/%Y')
        dif = (data_dt2 - data_dt1).days
        
        if dif < 0:
            print(data_dt1.date(), " : ", data_dt2.date()," : ",typeData)
            return False, dif,True
        
        return True, dif,False
    except ValueError as vs:
        #print(vs)
        # print("Error en alguna de las 2 fechas")
        return False , -1,False


def downloadData():
    """Shows basic usage of the Sheets API.
    Prints values from a sample spreadsheet.
    """
    creds = None
    # The file token.pickle stores the user's access and refresh tokens, and is
    # created automatically when the authorization flow completes for the first
    # time.
    if os.path.exists('token.pickle'):
        with open('token.pickle', 'rb') as token:
            creds = pickle.load(token)
    # If there are no (valid) credentials available, let the user log in.
    if not creds or not creds.valid:
        if creds and creds.expired and creds.refresh_token:
            creds.refresh(Request())
        else:
            flow = InstalledAppFlow.from_client_secrets_file(
                'credentials.json', SCOPES)
            creds = flow.run_local_server(port=0)
        # Save the credentials for the next run
        with open('token.pickle', 'wb') as token:
            pickle.dump(creds, token)

    service = build('sheets', 'v4', credentials=creds)

    # Call the Sheets API
    sheet = service.spreadsheets()
    result = sheet.values().get(spreadsheetId=SAMPLE_SPREADSHEET_ID,
                                range=SAMPLE_RANGE_NAME).execute()
    values = result.get('values', [])

    if not values:
        print('No data found.')
    else:
        
        tracers = []
        data_dif =[]
        count_data = []
        avg_list = []


        for row in values:
            
            if  len(row) > 33:
                # print('%s, %s, %s'  % (row[0], row[14], row[16]))
                # print('%s' % (row[24]))
                tracer = row[24]
                tracersNamesListed(tracers,tracer,data_dif,count_data)
                

        for row in values:
            
            if len(row) > 33:
                tracer = row[24]
                typeCase = row[3]
                folio = row[1]
                nombre = row[4]
                rut = row[5]

                if  typeCase.strip() == "TRAZADOR":

                    isValid, date_diference,negative = subtractDates(row[0],row[11],nombre)

                    if isValid or negative:
                        index = tracers.index(tracer)
                        count_data[index] = count_data[index] + 1
                        data_dif[index] = data_dif[index] + date_diference
                    
                            
        for i in range (len(tracers)): 
            avg = 0

            try:
                avg = data_dif[i] / count_data[i]
            except ZeroDivisionError as Zerror:
                avg = 0

            avg = round(avg,2)
            avg_list.append(avg)
            
        return tracers,avg_list
        
        
        
if __name__ == '__main__':

    tracers,avg_list =downloadData()

    # Delete Headers and null data from arrays
    index = tracers.index("")

    tracers.pop(index)
    tracers.pop(0) 

    avg_list.pop(index)
    avg_list.pop(0)

    # Dataframe
    df = pd.DataFrame({"TRAZADOR" : tracers ,"PROMEDIO DIAS": avg_list})
    print(df);

    # Export Dataframe
    df.to_csv('Promedio Trazadores '+datetime.now().strftime('%d-%m-%Y')+".csv", 
            encoding='utf-8', 
            index=False)

