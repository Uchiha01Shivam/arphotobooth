using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.Net.Mail;
using System.Net;
using System;

public class ImageLoader : MonoBehaviour
{
    public Image lastImage;

    // Specify the desired screenshot directory
     string imagePath = "Screenshots";

    // Start is called before the first frame update
    void Start()
    {
        LoadLastImage();
    }

    // Update is called once per frame
    void Update()
    {
        // You can add any update logic here if needed
    }

    void LoadLastImage()
    {
        // Create the full path by combining the persistent data path and the specified directory
        string fullImagePath = Path.Combine(Application.persistentDataPath, imagePath);

        // Check if the directory exists
        if (Directory.Exists(fullImagePath))
        {
            string[] imageFiles = Directory.GetFiles(fullImagePath, "*.png"); // Adjust the file extension as needed

            if (imageFiles.Length > 0)
            {
                string lastImagePath = imageFiles[imageFiles.Length - 1];
                byte[] imageData = File.ReadAllBytes(lastImagePath);

                Texture2D texture = new Texture2D(2, 2); // Adjust the size based on your image dimensions
                texture.LoadImage(imageData);

                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

                lastImage.sprite = sprite;
            }
            else
            {
                Debug.LogWarning("No images found in the specified path.");
            }
        }
        else
        {
            Debug.LogWarning("The specified directory does not exist.");
        }
    }
    public void shareToWhatsApp()
    {
        // Get the WhatsApp number from PlayerPrefs
        string number = PlayerPrefs.GetString("number");

        // Ensure there is a WhatsApp number stored
        if (!string.IsNullOrEmpty(number))
        {
            // Get the path of the last loaded image
            string fullImagePath = GetLastImagePath();

            // Check if the image exists
            if (!string.IsNullOrEmpty(fullImagePath) && File.Exists(fullImagePath))
            {
                // Construct the share message
                string shareMessage = "Check out this image!";

                // You may need to adjust the URL scheme based on your platform
                string urlScheme = "https://web.whatsapp.com/send?text=" + WWW.EscapeURL(shareMessage) + "&phone=" + WWW.EscapeURL(number);

                // Open the default sharing dialog
                Application.OpenURL(urlScheme);
            }
            else
            {
                Debug.LogWarning("Image not found. Please load an image first.");
            }
        }
        else
        {
            Debug.LogWarning("WhatsApp number not found in PlayerPrefs.");
        }
    }

    public void MailLastImage()
    {
        // Get the path of the last loaded image
        string fullImagePath = GetLastImagePath();

        // Check if the image exists
        if (!string.IsNullOrEmpty(fullImagePath) && File.Exists(fullImagePath))
        {
            // Create a new mail message
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("shivamacharya1507@gmail.com"); // Change this to your email address
            mail.To.Add("acharyashivam880@gmail.com");
            mail.Subject = "Last Loaded Image";
            mail.Body = "Please find the last loaded image attached.";

            // Attach the image file to the mail
            Attachment attachment = new Attachment(fullImagePath);
            mail.Attachments.Add(attachment);

            // Setup SMTP client
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new NetworkCredential("shivamacharya1507@gmail.com", "arajnmpsjckbgjhg"); // Change this to your email and password
            smtpServer.EnableSsl = true;

            try
            {
                // Send the mail
                smtpServer.Send(mail);
                Debug.Log("Mail sent successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to send mail: " + ex.Message);
            }
            finally
            {
                // Dispose attachments and mail objects
                attachment.Dispose();
                mail.Dispose();
            }
        }
        else
        {
            Debug.LogWarning("Image not found. Please load an image first.");
        }
    }

    // Helper method to get the path of the last loaded image
    private string GetLastImagePath()
    {
        string fullImagePath = "";

        // Specify the desired screenshot directory
        string imagePath = "Screenshots";

        // Create the full path by combining the persistent data path and the specified directory
        string fullScreenshotPath = Path.Combine(Application.persistentDataPath, imagePath);

        // Check if the directory exists
        if (Directory.Exists(fullScreenshotPath))
        {
            string[] imageFiles = Directory.GetFiles(fullScreenshotPath, "*.png"); // Adjust the file extension as needed

            if (imageFiles.Length > 0)
            {
                // Get the path of the last image
                fullImagePath = imageFiles[imageFiles.Length - 1];
            }
        }

        return fullImagePath;
    }
    // Public function to delete the last loaded image
    public void DeleteLastImage()
    {
        // Create the full path by combining the persistent data path and the specified directory
        string fullImagePath = Path.Combine(Application.persistentDataPath, imagePath);

        // Check if the directory exists
        if (Directory.Exists(fullImagePath))
        {
            string[] imageFiles = Directory.GetFiles(fullImagePath, "*.png"); // Adjust the file extension as needed

            if (imageFiles.Length > 0)
            {
                string lastImagePath = imageFiles[imageFiles.Length - 1];
                File.Delete(lastImagePath);
                Debug.Log("Last image deleted.");
                LoadLastImage(); // Reload the updated list of images
            }
            else
            {
                Debug.LogWarning("No images found in the specified path.");
            }
        }
        else
        {
            Debug.LogWarning("The specified directory does not exist.");
        }
    }
    public void mainmenu()
    {
        SceneManager.LoadScene(0);
    }
}
