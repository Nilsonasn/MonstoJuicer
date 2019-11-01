from __future__ import print_function
import os
import subprocess
import pandas as pd
import numpy as np
from sklearn.tree import DecisionTreeClassifier
from sklearn.metrics import classification_report, confusion_matrix 
from sklearn.model_selection import train_test_split
from sklearn import preprocessing
from sklearn.preprocessing import StandardScaler 
from sklearn.neural_network import MLPClassifier
from sklearn.neural_network import MLPRegressor
from sklearn.tree import DecisionTreeRegressor
from sklearn import linear_model
pd.options.mode.chained_assignment = None  # default='warn'

df = pd.read_csv("Monstos.csv")



features = ["name","armor class","hit points","strength","dexterity","constitution","intelligence","wisdom","charisma","constitution_save","intelligence_save","wisdom_save","Special Abilities","actions","legendary_actions"]
feature_data = ["armor class","hit points","strength","dexterity","constitution","intelligence","wisdom","charisma","Special Abilities","actions","legendary_actions"]

target = df["challenge_rating"]
data = df[features]


X_train, X_test, y_train, y_test = train_test_split(data, target, test_size=.1)


dt = DecisionTreeRegressor(min_samples_split=40, random_state=99)
dt.fit(X_train[feature_data], y_train)

mlp = MLPRegressor(hidden_layer_sizes=(20, 20, 20), max_iter=2000)  
mlp.fit(X_train[feature_data], y_train)

sgd = linear_model.SGDRegressor(max_iter=1000, tol=1e-3)
sgd.fit(X_train[feature_data], y_train)

monster = {"armor class":[16],"hit points":[50],"strength":[10],"dexterity":[10],"constitution":[20],"intelligence":[10],"wisdom":[6],"charisma":[8],"Special Abilities":[2],"actions":[2],"legendary_actions":[1]}
monster = pd.DataFrame(monster)

preds = mlp.predict(monster)
print(preds)
print()
preds = mlp.predict(X_test[feature_data])
predictions = X_test
predictions["prediction"] = preds
predictions["actual"]=y_test
print(predictions[["name","prediction","actual"]])

print()

preds = sgd.predict(monster)
print(preds)
print()
preds = sgd.predict(X_test[feature_data])
predictions = X_test
predictions["prediction"] = preds
predictions["actual"]=y_test
print(predictions[["name","prediction","actual"]])




