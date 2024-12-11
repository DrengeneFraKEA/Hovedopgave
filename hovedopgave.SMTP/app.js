const express = require("express");
const bodyParser = require("body-parser");
const nodemailer = require("nodemailer");

const app = express();
app.use(bodyParser.json());

app.post("/send-email", async (req, res) => {
    const { to, subject, text } = req.body;

    const transporterMailpit = nodemailer.createTransport({
        host: "localhost",
        port: 1025,
        secure: false,
    });

    const transporterGmail = nodemailer.createTransport( {
        host: "smtp.gmail.com",
        port: 587,                 
        secure: false,             
        auth: {
            user: "drengenefrakea@gmail.com", 
            pass: "iafa urfe opwy wisz",     
        },

    })

    const mailOptions = {
        from: 'drengenefrakea@gmail.com',
        to,
        subject,
        text,
    };

    try {
        // const info = await transporterMailpit.sendMail(mailOptionsMailpit);
        const info = await transporterGmail.sendMail(mailOptions)
        console.log("Email sent:", info.messageId);
        res.status(200).send({ message: "Email sent successfully" });
    } catch (error) {
        console.error("Error sending email:", error);
        res.status(500).send({ message: "Error sending email", error });
    }
});

app.listen(3000, () => {
    console.log("SMTP server listening on port 3000");
});
