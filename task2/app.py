from flask import Flask,render_template,flash,make_response, redirect,url_for,session,logging,request
from flask_sqlalchemy import SQLAlchemy
import hashlib, smtplib, sqlite3, random
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText

app = Flask(__name__)
app.config['SECRET_KEY']='825643f8439b2ea37a1593ae03293021bdf532d1'

@app.route("/")
def index():
    session['data'] = dict(login='False')
    return render_template("index.html")

@app.route("/email_suc", methods=["GET", "POST"])
def email_suc():
    if session.get('data')['login'] == 'Process':
        connect = sqlite3.connect("database1.db")
        uname = request.args.get('uname')
        if request.method == "POST":
            if request.form["btn_identifier"] == "send":
                cursor = connect.cursor()
                cursor.execute("SELECT email FROM users WHERE username=(?)", (uname,))
                umail= cursor.fetchone()
                connect.commit()
                rand= random.randint(9999,99999)
                cursor = connect.cursor()
                cursor.execute("UPDATE users SET temp = (?) WHERE username = (?)", (rand,uname))
                connect.commit()
                sendmail(umail,rand)
            else:
                temp = request.form["temp"]
                cursor = connect.cursor()
                cursor.execute ("SELECT temp from users WHERE (temp = (?) AND username = (?))", (temp,uname))
                temp_pass = cursor.fetchone()
                connect.commit()
                try:
                    if temp_pass[0] == temp:
                        session['data'] = dict(username=uname, login='True')
                        return redirect(url_for("lk"))
                except TypeError:
                    return render_template("email_suc.html", wrong_message="Неверный временный пароль")
    else:
        return redirect(url_for('login'))
    return render_template("email_suc.html")

def sendmail(addr_to, temp_pass):
   
    msg = MIMEMultipart()
    body = "Ваш пароль " + str(temp_pass) + " действует в течении 300 секунд"
    msg['Subject'] = 'Временный пароль'
    msg.attach(MIMEText(body, 'plain'))              
    server = smtplib.SMTP('smtp.gmail.com', 587) 
    server.starttls()
    server.login("flssk.elm@gmail.com", "Qq@123456" )
    server.sendmail("flssk.elm@gmail.com", addr_to, msg.as_string())
    server.quit()                            

@app.route('/lk')
def lk():
        if session.get('data')['login'] == 'True':
            username = session['data']['username']
            return render_template('lk.html', name=username)
        else:
            return redirect(url_for('login'))

@app.route("/login",methods=["GET", "POST"])
def login():
    if request.method == "POST":
        uname = request.form["uname"]
        passw = request.form["passw"]

        connect = sqlite3.connect("database1.db")
        cursor = connect.cursor()
        cursor.execute ("SELECT userid FROM users WHERE (username=(?) and password = (?))",(uname, passw))
        userid_t = cursor.fetchone()
        connect.commit()
        try:
            if userid_t[0] is not None:
                session['data'] = dict(username=uname, login='Process')
                return redirect(url_for("email_suc", uname=uname))
        except:
            return render_template("login.html", wrong_message="Неверный логин или пароль")
    return render_template("login.html")


@app.route("/register", methods=["GET", "POST"])
def register():
    message=''
    if request.method == "POST":
        uname = request.form['uname']
        mail = request.form['mail']
        passw = request.form['passw']

        connect = sqlite3.connect("database1.db")
        cursor = connect.cursor()
        cursor.execute ("SELECT MAX(userid) FROM users;")
        userid_t = cursor.fetchone()
        userid = int(userid_t[0])+1
        rand = random.randint(9999,99999)
        cursor.execute("""INSERT INTO users(userid, username, password, email, temp) 
            VALUES((?), (?), (?), (?), (?));""", (userid,uname,passw,mail,rand))
        connect.commit()

        return redirect(url_for("login"))
    return render_template("register.html", message=message)


if __name__ == "__main__":
    app.run()