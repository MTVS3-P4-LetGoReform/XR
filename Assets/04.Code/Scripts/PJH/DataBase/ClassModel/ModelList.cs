using System;
using System.Collections.Generic;

[Serializable]
public class ModelList
{
    public Dictionary<string, Model> models = new Dictionary<string, Model>();

    public ModelList() { }

    public bool AddModel(Model model)
    {
        if (models.ContainsKey(model.id))
        {
            Console.WriteLine($"Model with id {model.id} already exists.");
            return false;
        }
        models[model.id] = model;
        return true;
    }

    public bool RemoveModel(string id)
    {
        return models.Remove(id);
    }

    public Model GetModel(string id)
    {
        models.TryGetValue(id, out var model);
        return model;
    }

    public List<Model> GetAllModels()
    {
        return new List<Model>(models.Values);
    }
}