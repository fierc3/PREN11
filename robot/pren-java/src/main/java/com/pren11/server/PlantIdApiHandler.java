package com.pren11.server;

import com.pren11.Config;
import org.json.JSONArray;
import org.json.JSONObject;

import java.io.InputStream;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.Base64;
import java.util.Iterator;

public class PlantIdApiHandler {


    private static String base64EncodeFromBytes(byte[] file) throws Exception {
        String res = Base64.getEncoder().encodeToString(file);
        return res;
    }

    private static String sendPostRequest(String urlString, JSONObject data) throws Exception {
        URL url = new URL(urlString);
        HttpURLConnection con = (HttpURLConnection) url.openConnection();

        con.setDoOutput(true);
        con.setDoInput(true);
        con.setRequestMethod("POST");
        con.setRequestProperty("Content-Type", "application/json");

        OutputStream os = con.getOutputStream();
        os.write(data.toString().getBytes());
        os.close();

        InputStream is = con.getInputStream();
        String response = new String(is.readAllBytes());

//        System.out.println("Response code : " + con.getResponseCode());
//        System.out.println("Response : " + response);
        con.disconnect();
        return response;
    }

    public static PlantIdResult sendToPlantId(byte[] array) throws Exception {
        var result = new PlantIdResult("Unknown","",100);
        if(Config.OFFLINE_MODE) return result;
        if(!Config.SENDTOPLANTID){
            System.out.println("NOT SENDING to plantid, returning mock value");
            return result;
        }
        String apiKey = Config.API_PLANTID;


        JSONObject data = new JSONObject();
        data.put("api_key", apiKey);
        String fileData = base64EncodeFromBytes(array);
        JSONArray images = new JSONArray();
        images.put(fileData);
        data.put("images", images);

        // add modifiers
        // modifiers info: https://github.com/flowerchecker/Plant-id-API/wiki/Modifiers
        JSONArray modifiers = new JSONArray()
                .put("crops_fast")
                .put("similar_images");
        data.put("modifiers", modifiers);

        // add language
        data.put("plant_language", "en");

        // add plant details
        // more info here: https://github.com/flowerchecker/Plant-id-API/wiki/Plant-details
        JSONArray plantDetails = new JSONArray()
                .put("common_names")
                .put("url")
                .put("name_authority")
                .put("wiki_description")
                .put("taxonomy")
                .put("synonyms");
        data.put("plant_details", plantDetails);

        var res = sendPostRequest(Config.URL_PLANTID, data);
        //System.out.println("PlantId Result: \n" + res);
        JSONObject jsonObject = new JSONObject(res.trim());
        Iterator<String> keys = jsonObject.keys();

        while(keys.hasNext()) {
            String key = keys.next();
            if (jsonObject.get(key) instanceof JSONArray) {

                if(key.equals("suggestions")){
                    var arraySuggestions = jsonObject.getJSONArray(key);
                    var firstSuggestion = arraySuggestions.getJSONObject(0);
                    var details  =firstSuggestion.getJSONObject("plant_details");
                    var plantName = firstSuggestion.getString("plant_name");
                    try {
                        var commonNames = details.getJSONArray("common_names");
                        if(commonNames.length() > 0){
                            plantName = commonNames.getString(0);
                        }
                    }catch (Exception ex){
                        System.out.println("Failed to get common names, ex: " + ex.getLocalizedMessage());
                    }
                    System.out.println("PlantId Returned: " + plantName);
                    result.setPlantName(plantName);

                    try{
                        var similarImages = firstSuggestion.getJSONArray("similar_images");
                        if(similarImages.length() > 0){
                            result.setImageUrl(((JSONObject)similarImages.get(0)).getString("url"));
                        }
                    }catch (Exception ex){
                        System.err.println("failed to get image for plant: "+ ex.getLocalizedMessage());
                    }

                    try{
                        var p = firstSuggestion.getDouble("probability");
                        result.setProbability(p);
                    }catch (Exception ex){
                        System.err.println("failed to get probability for plant: "+ ex.getLocalizedMessage());
                    }
                }
            }
        }
        return result;
    }
}
