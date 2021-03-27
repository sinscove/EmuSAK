import flask
import json
from flask import jsonify ,request ,send_file
from flask_cors import CORS
import requests
import xml.etree.ElementTree as ET
import os
import asyncio
root = ET.fromstring(open("nsw.xml","r",encoding="utf-8").read())
app = flask.Flask(__name__)
cors= CORS(app)
app.config["DEBUG"] = True
version='1.0'
gamesDB=[]
def getShaders():
    global gamesDB,root,availableShadersRyu,availableShadersYuzu
    gamesDB={}
    availableShadersRyu={}
    availableShadersYuzu={}
    for child in root:
        gamesDB[child[8].text.replace(" (v0)","")]=child[1].text
    for file in os.listdir(os.getcwd()+r"\\shaders\\ryu"):
        if file.endswith(".zip"):
            availableShadersRyu[gamesDB[file.replace(".zip","")]]=file.replace(".zip","")

    for file in os.listdir(os.getcwd()+r"\\shaders\\yuzu"):
        if file.endswith(".bin"):
            availableShadersYuzu[gamesDB[file.replace(".bin","")]]=file.replace(".bin","")

getShaders()
def getSaves():
    global saves,gamesDB
    saves={}
    for file in os.listdir(os.getcwd()+r"\\saves"):
        saves[file]=os.listdir(os.getcwd()+r"\\saves\\"+file)[0]

getSaves()

@app.route('/api/version', methods=['GET'])
def home():
    return version

@app.route('/api/keys', methods=['GET'])
def getKeys():
    return open("prod.keys","r").read()

@app.route('/api/firmware', methods=['GET'])
def getFirmware():
    return send_file("firmware.zip")

        
@app.route('/api/shaders/ryujinx', methods=['GET'])
def getShader():
    global shaders,availableShaders
    if(gamesDB==[]):
        getShaders()
    if(request.args.get('id')!=None):
        if(request.args.get("type")=="zip"):
            id=request.args.get('id')
            for i in availableShadersRyu:
                if(availableShadersRyu[i]==id):
                    return send_file(r"shaders/ryu/"+id+".zip",as_attachment=True)
            return "Couldn't find game"
        elif(request.args.get("type")=="info"):
            id=request.args.get('id')
            for i in availableShadersRyu:
                if(availableShadersRyu[i]==id):
                    return send_file(r"shaders/ryu/"+id+".info",as_attachment=True)
            return "Couldn't find game"        
        else:
            return "Parsing error"
    else:
        return jsonify(availableShadersRyu)
@app.route('/api/shaders/yuzu', methods=['GET'])
def getShaderYuzu():
    global shaders,availableShadersYuzu
    if(gamesDB==[]):
        getShaders()
    if(request.args.get('id')!=None):
        id=request.args.get('id')
        for i in availableShadersYuzu:
            if(availableShadersYuzu[i]==id):
                return send_file(r"shaders/yuzu/"+id+".bin",as_attachment=True)
        return "Couldn't find game"

    else:
        return jsonify(availableShadersYuzu)
@app.route('/api/saves', methods=['GET'])
def getSaves():
    global saves
    if(request.args.get('id')!=None):
        for i in saves:
            if(request.args.get("id")==i):
                return send_file("saves/"+i+"/"+saves[i],as_attachment=True)
        return "File not found"
    return jsonify(saves)
app.run()
