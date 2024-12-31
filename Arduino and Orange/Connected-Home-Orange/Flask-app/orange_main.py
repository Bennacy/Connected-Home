# supported on Python 3.10
# pip install setuptools wheel Orange3 Orange3-Associate Orange3-Text PyQt5
from flask import Flask, request, jsonify
import Orange
import pickle
import os
import numpy as np
import os

# Get the directory of the current script
current_dir = os.path.dirname(os.path.abspath(__file__))
parent_folder = os.path.abspath(os.path.join(current_dir, os.pardir))

# Set the model path dynamically
treeModelPath = os.path.join(current_dir, 'treeModel.pkcls')
logisticRegressionPath = os.path.join(parent_folder, 'Logistic_Regression_Model.pkcls')
print(current_dir)
print(parent_folder)
print(logisticRegressionPath)

app = Flask(__name__)

# # Load the trained model
# with open(treeModelPath, 'rb') as model_file:
#     treeModel = pickle.load(model_file)

with open(logisticRegressionPath, 'rb') as model_file:
    logisticRegressionModel = pickle.load(model_file)

import numpy as np

@app.route('/')
def hello_world():
    print("A")
    return ''

# # @app.route('/predictDecisionTree', methods=['POST'])
# # def predictDecisionTree():
# #     try:
# #         # Parse JSON request
# #         data = request.json
# #         features = data['features']  # Extract features from JSON

# #         # Convert features to Orange.data.Table
# #         input_data = Orange.data.Table.from_list(domain=treeModel.domain, rows=[features])

# #         # Predict using the decision tree model
# #         prediction = treeModel(input_data)

# #         # Map numeric prediction to class name
# #         predicted_index = prediction[0]  # Numeric index of the class
# #         predicted_class = treeModel.domain.class_var.values[int(predicted_index)]

# #         # Return the predicted class as JSON
# #         return jsonify({'prediction': predicted_class})

# #     except Exception as e:
# #         # Handle errors and provide feedback
# #         return jsonify({'error': str(e)}), 400

    
@app.route('/predictLogisticRegression', methods=['POST'])
def predictLogisticRegression():
    try:
        # Parse JSON request
        data = request.json
        features = data['features']  # Extract features from JSON
        print(features)

        # Convert features to Orange.data.Table
        input_data = Orange.data.Table.from_list(domain=logisticRegressionModel.domain, rows=[features])

        # Predict using the logistic regression model
        prediction = logisticRegressionModel(input_data)

        # Map numeric prediction to class name
        predicted_index = prediction[0]  # Numeric index of the class
        predicted_class = logisticRegressionModel.domain.class_var.values[int(predicted_index)]
        print(predicted_class)

        # Return the predicted class as JSON
        return jsonify({'prediction': predicted_class})

    except Exception as e:
        # Handle errors and provide feedback
        return jsonify({'error': str(e)}), 400


# os.system("cls")
# print("\n\n\n\n\n\n\n\n\n\n\n")
if __name__ == '__main__':
    app.run()

