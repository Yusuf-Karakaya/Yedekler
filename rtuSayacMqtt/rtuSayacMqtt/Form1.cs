using System.IO.Ports;
using System.Text;
using Modbus.Device;
using Modbus.Utility;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace rtuSayac
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort = null;
        private string temp;
        private int sayac = 0;
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            byte slaveAdres = 95;
            ushort baslamaAdres = 1;
            ushort boyut = 12;
            try
            {
                IModbusMaster masterRtu = ModbusSerialMaster.CreateRtu(serialPort);
                ushort[] cikti = masterRtu.ReadHoldingRegisters(slaveAdres, baslamaAdres, boyut);
                //----------------------------------------------------------------------
                //ProgressBar deðer atamalarý Rtu sayaç üzerinden çekilen veriler dahilinde.
                //----------------------------------------------------------------------
                watsaatYuksek.Value = cikti[3];
                watsaatAlcak.Value = cikti[4];
                gerilimProgressbar.Value = Convert.ToInt32(cikti[5]) / 100;
                akimProgressbar.Value = cikti[6];
                frekansProgressbar.Value = cikti[7];
                powerfactorProgressbar.Value = cikti[8];
                aktifgucProgressbar.Value = cikti[9];
                reaktifgucProgressbar.Value = (int)cikti[10];
                gorunurgucProgressbar.Value = cikti[11];
                //----------------------------------------------------------------------

                //----------------------------------------------------------------------
                //ProgressBar içerisindeki textlere rtu sayaç üzerinden çekilen verilen atýlmasý.
                //----------------------------------------------------------------------
                wattSaat_yuksekGerilim.Text = Convert.ToString(cikti[3]);
                watSaat_dusukGerilim.Text = String.Format("{0} ", cikti[4]);
                gerilim.Text = String.Format("{0} ", Convert.ToInt32(cikti[5]) / 100);
                akim.Text = String.Format("{0} ", cikti[6]);
                frekans.Text = String.Format("{0} ", Convert.ToInt32(cikti[7]) / 100);
                powerFactor.Text = String.Format("{0} ", Convert.ToInt32(cikti[8]) / 1000);
                aktifGuc.Text = String.Format("{0} ", Convert.ToInt32(cikti[9]) / 100);
                reaktifGuc.Text = String.Format("{0} ", Convert.ToInt32(cikti[10]) / 100);
                //----------------------------------------------------------------------

                //----------------------------------------------------------------------
                //Modbus Adresini Textboxa yazdýrma iþlemi
                //----------------------------------------------------------------------
                //

                foreach (ushort veri in cikti)
                {
                    textBox1.Text += String.Format("{0}/ ", veri);
                    sayac++;
                }
                if (sayac % 12 == 0)
                {
                    temp = textBox1.Text;
                    FileStream fs = new FileStream(@"C:\Users\ykarakaya\Desktop\Holding Register.txt", FileMode.Append, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(temp);
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
                modbusAdresi.Text = Convert.ToString(cikti[0], 16);


            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                throw;
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //---------------------------------------
            //form ilk ekrana geldiðinde gerekli kontrollerin yapýlmasý için 
            //butonlarýn enable özellikleri ve checkhboxýn görünürlüðü false deðeri verilerek butonlara basýlmasý engellenir. 
            holdingRegOku.Enabled = false;
            inputRegister.Enabled = false;
            baglantiKes.Enabled = false;
            canliVeri.Visible = false;
            //---------------------------------------
            //---------------------------------------
            //
            string[] portlar = SerialPort.GetPortNames();
            foreach (string portAdi in portlar)
            {
                portNum.Items.Add(portAdi);
            }
            //---------------------------------------

        }

        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                serialPort = new SerialPort(Convert.ToString(portNum.SelectedItem), Convert.ToInt32(baudRate.SelectedItem), Parity.None, 8, StopBits.One);


                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                throw;
            }
            holdingRegOku.Enabled = true;
            inputRegister.Enabled = true;
            button2.Enabled = false;
            baglantiKes.Enabled = true;
            canliVeri.Visible = true;


        }

        private void baglantiKes_Click(object sender, EventArgs e)
        {
            serialPort.Close();

            button2.Enabled = true;
            holdingRegOku.Enabled = false;
            inputRegister.Enabled = false;
            timer1.Stop();
            watsaatYuksek.Value = 0;
            watsaatAlcak.Value = 0;
            gerilimProgressbar.Value = 0;
            akimProgressbar.Value = 0;
            frekansProgressbar.Value = 0;
            powerfactorProgressbar.Value = 0;
            aktifgucProgressbar.Value = 0;
            reaktifgucProgressbar.Value = 0;
            gorunurgucProgressbar.Value = 0;
            wattSaat_yuksekGerilim.Text = Convert.ToString("0");
            watSaat_dusukGerilim.Text = Convert.ToString("0");
            gerilim.Text = Convert.ToString("0");
            akim.Text = Convert.ToString("0");
            powerFactor.Text = Convert.ToString("0");
            frekans.Text = Convert.ToString("0");
            aktifGuc.Text = Convert.ToString("0");
            reaktifGuc.Text = Convert.ToString("0");
            gorunurGuc.Text = Convert.ToString("0");
            modbusAdresi.Clear();
            textBox1.Clear();
            portNum.SelectedIndex = -1;
            baudRate.SelectedIndex = -1;
            canliVeri.Visible = false;
            canliVeri.Checked = false;
            guna2ToggleSwitch1.Visible = false;
            role.Visible = false;
            fan.Visible = false;
            fanToggler.Visible = false;

        }

        private void canliVeri_CheckedChanged(object sender, EventArgs e)
        {
            //------------------------
            //canli veri checkboxý deðiþtiðinde eðer chexbox iþaretli ise timer 200 ms gecikme ile timer fonk. çalýþtýrýlýr. 
            //------------------------
            if (canliVeri.Checked)
            {
                timer1.Interval = 200;
                timer1.Start();

            }
            //-------canli veri iþaretli deðilse timer durdurulur.
            if (!(canliVeri.Checked))
            {
                timer1.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //---------------------------------------
            //timer canli veri check olduðunda holding registerlarýný okuyan butonu çalýþtýrýr.
            //Bu þekilde buton döngü þeklinde çalýþmýþ olur.
            //---------------------------------------
            button1_Click(sender, e);
        }

        private void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            IModbusMaster masterRtuToggle = ModbusSerialMaster.CreateRtu(serialPort);

            if (guna2ToggleSwitch1.Checked == true)
            {
                masterRtuToggle.WriteSingleCoil(1, 1, true);
            }
            else
            {
                masterRtuToggle.WriteSingleCoil(1, 1, false);
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.Select(textBox1.TextLength, 0);
            textBox1.ScrollToCaret();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            IModbusMaster masterRtuInput = ModbusSerialMaster.CreateRtu(serialPort);
            ushort[] ciktiInput = masterRtuInput.ReadInputRegisters(1, 0, 6);
            foreach (ushort veri in ciktiInput)
            {
                textBox1.Text += String.Format("{0}/ ", veri);
                sayac++;
            }
            if (sayac % 6 == 0)
            {
                temp = textBox1.Text;

                FileStream fs = new FileStream(@"C:\Users\ykarakaya\Desktop\Input Register.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine(temp);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private void guna2ToggleSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            IModbusMaster masterRtuToggleFan = ModbusSerialMaster.CreateRtu(serialPort);

            if (fanToggler.Checked == true)
            {
                masterRtuToggleFan.WriteSingleCoil(1, 0, true);
            }
            else
            {
                masterRtuToggleFan.WriteSingleCoil(1, 0, false);
            }
        }

        private void temizle_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }





        private void publishMqtt_Click(object sender, EventArgs e)
        {
            MqttClient mqttClient = new MqttClient("broker.hivemq.com");
            string clientId = Guid.NewGuid().ToString();


            mqttClient.Connect(clientId);

            /*
            Console.WriteLine("Abone olundu : test/deneme");
            Console.WriteLine("mesaj bekleniyor.");
            */

            string message = "0";
            string topic = "test/deneme3";
            mqttClient.Publish(topic, Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }

        private void receiveMqtt_Click(object sender, EventArgs e)
        {
            MqttClient mqttClientRecieve = new MqttClient("broker.hivemq.com");
            mqttClientRecieve.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
            string clientId = Guid.NewGuid().ToString();


            mqttClientRecieve.Connect(clientId);
            // Console.WriteLine("Abone olundu : test/deneme");
            mqttClientRecieve.Subscribe(new string[] { "test/deneme" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            //birden fazla topic varsa hata veiyormuþ

        }
        private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            IModbusMaster masterRtuTogg = ModbusSerialMaster.CreateRtu(serialPort);

            var message = Encoding.Default.GetString(e.Message);
            if (message == "1")
            {
                masterRtuTogg.WriteSingleCoil(1, 0, true);
            }
            if (message == "0")
            {

                masterRtuTogg.WriteSingleCoil(1, 0, false);

            }

        }
        private Label gerilim;
        private Label gorunurGuc;
        private Label powerFactor;
        private Label reaktifGuc;
        private Label aktifGuc;
        private Label frekans;
        private Label akim;
        private Label watSaat_dusukGerilim;
        private Label wattSaat_yuksekGerilim;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gerilim = new System.Windows.Forms.Label();
            this.gorunurGuc = new System.Windows.Forms.Label();
            this.powerFactor = new System.Windows.Forms.Label();
            this.reaktifGuc = new System.Windows.Forms.Label();
            this.aktifGuc = new System.Windows.Forms.Label();
            this.frekans = new System.Windows.Forms.Label();
            this.akim = new System.Windows.Forms.Label();
            this.watSaat_dusukGerilim = new System.Windows.Forms.Label();
            this.wattSaat_yuksekGerilim = new System.Windows.Forms.Label();
            this.receiveMqtt = new System.Windows.Forms.Button();
            this.publishMqtt = new System.Windows.Forms.Button();
            this.temizle = new System.Windows.Forms.Button();
            this.fan = new System.Windows.Forms.Label();
            this.fanToggler = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.inputRegister = new System.Windows.Forms.Button();
            this.role = new System.Windows.Forms.Label();
            this.guna2ToggleSwitch1 = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.canliVeri = new Guna.UI2.WinForms.Guna2CheckBox();
            this.baglantiKes = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.baudRate = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.portNum = new Guna.UI2.WinForms.Guna2ComboBox();
            this.portNo = new System.Windows.Forms.Label();
            this.gerilimProgressbar = new Guna.UI2.WinForms.Guna2CircleProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.gorunurgucProgressbar = new Guna.UI2.WinForms.Guna2CircleProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.powerfactorProgressbar = new Guna.UI2.WinForms.Guna2CircleProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.reaktifgucProgressbar = new Guna.UI2.WinForms.Guna2CircleProgressBar();
            this.label5 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.aktifgucProgressbar = new Guna.UI2.WinForms.Guna2CircleProgressBar();
            this.label6 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.frekansProgressbar = new Guna.UI2.WinForms.Guna2CircleProgressBar();
            this.label7 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.akimProgressbar = new Guna.UI2.WinForms.Guna2CircleProgressBar();
            this.label8 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.watsaatAlcak = new Guna.UI2.WinForms.Guna2CircleProgressBar();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.watsaatYuksek = new Guna.UI2.WinForms.Guna2CircleProgressBar();
            this.label10 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.modbusAdresi = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.holdingRegOku = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.gerilimProgressbar.SuspendLayout();
            this.gorunurgucProgressbar.SuspendLayout();
            this.powerfactorProgressbar.SuspendLayout();
            this.reaktifgucProgressbar.SuspendLayout();
            this.aktifgucProgressbar.SuspendLayout();
            this.frekansProgressbar.SuspendLayout();
            this.akimProgressbar.SuspendLayout();
            this.watsaatAlcak.SuspendLayout();
            this.watsaatYuksek.SuspendLayout();
            this.SuspendLayout();
            // 
            // gerilim
            // 
            this.gerilim.AutoSize = true;
            this.gerilim.BackColor = System.Drawing.SystemColors.Control;
            this.gerilim.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.gerilim.ForeColor = System.Drawing.Color.Black;
            this.gerilim.Location = new System.Drawing.Point(-133, 16);
            this.gerilim.Margin = new System.Windows.Forms.Padding(0);
            this.gerilim.Name = "gerilim";
            this.gerilim.Size = new System.Drawing.Size(26, 27);
            this.gerilim.TabIndex = 65;
            this.gerilim.Text = "0";
            this.gerilim.UseWaitCursor = true;
            // 
            // gorunurGuc
            // 
            this.gorunurGuc.AutoSize = true;
            this.gorunurGuc.BackColor = System.Drawing.SystemColors.Control;
            this.gorunurGuc.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.gorunurGuc.ForeColor = System.Drawing.Color.Black;
            this.gorunurGuc.Location = new System.Drawing.Point(-133, 16);
            this.gorunurGuc.Name = "gorunurGuc";
            this.gorunurGuc.Size = new System.Drawing.Size(26, 27);
            this.gorunurGuc.TabIndex = 75;
            this.gorunurGuc.Text = "0";
            this.gorunurGuc.UseWaitCursor = true;
            // 
            // powerFactor
            // 
            this.powerFactor.AutoSize = true;
            this.powerFactor.BackColor = System.Drawing.SystemColors.Control;
            this.powerFactor.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.powerFactor.ForeColor = System.Drawing.Color.Black;
            this.powerFactor.Location = new System.Drawing.Point(-133, 16);
            this.powerFactor.Name = "powerFactor";
            this.powerFactor.Size = new System.Drawing.Size(26, 27);
            this.powerFactor.TabIndex = 73;
            this.powerFactor.Text = "0";
            this.powerFactor.UseWaitCursor = true;
            // 
            // reaktifGuc
            // 
            this.reaktifGuc.AutoSize = true;
            this.reaktifGuc.BackColor = System.Drawing.SystemColors.Control;
            this.reaktifGuc.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.reaktifGuc.ForeColor = System.Drawing.Color.Black;
            this.reaktifGuc.Location = new System.Drawing.Point(-134, 16);
            this.reaktifGuc.Name = "reaktifGuc";
            this.reaktifGuc.Size = new System.Drawing.Size(26, 27);
            this.reaktifGuc.TabIndex = 74;
            this.reaktifGuc.Text = "0";
            this.reaktifGuc.UseWaitCursor = true;
            // 
            // aktifGuc
            // 
            this.aktifGuc.AutoSize = true;
            this.aktifGuc.BackColor = System.Drawing.SystemColors.Control;
            this.aktifGuc.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.aktifGuc.ForeColor = System.Drawing.Color.Black;
            this.aktifGuc.Location = new System.Drawing.Point(-133, 16);
            this.aktifGuc.Name = "aktifGuc";
            this.aktifGuc.Size = new System.Drawing.Size(26, 27);
            this.aktifGuc.TabIndex = 72;
            this.aktifGuc.Text = "0";
            this.aktifGuc.UseWaitCursor = true;
            // 
            // frekans
            // 
            this.frekans.AutoSize = true;
            this.frekans.BackColor = System.Drawing.SystemColors.Control;
            this.frekans.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.frekans.ForeColor = System.Drawing.Color.Black;
            this.frekans.Location = new System.Drawing.Point(-134, 17);
            this.frekans.Name = "frekans";
            this.frekans.Size = new System.Drawing.Size(26, 27);
            this.frekans.TabIndex = 71;
            this.frekans.Text = "0";
            this.frekans.UseWaitCursor = true;
            // 
            // akim
            // 
            this.akim.AutoSize = true;
            this.akim.BackColor = System.Drawing.SystemColors.Control;
            this.akim.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.akim.ForeColor = System.Drawing.Color.Black;
            this.akim.Location = new System.Drawing.Point(-133, 17);
            this.akim.Name = "akim";
            this.akim.Size = new System.Drawing.Size(26, 27);
            this.akim.TabIndex = 70;
            this.akim.Text = "0";
            this.akim.UseWaitCursor = true;
            // 
            // watSaat_dusukGerilim
            // 
            this.watSaat_dusukGerilim.AutoSize = true;
            this.watSaat_dusukGerilim.BackColor = System.Drawing.SystemColors.Control;
            this.watSaat_dusukGerilim.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.watSaat_dusukGerilim.ForeColor = System.Drawing.Color.Black;
            this.watSaat_dusukGerilim.Location = new System.Drawing.Point(-134, 16);
            this.watSaat_dusukGerilim.Name = "watSaat_dusukGerilim";
            this.watSaat_dusukGerilim.Size = new System.Drawing.Size(26, 27);
            this.watSaat_dusukGerilim.TabIndex = 67;
            this.watSaat_dusukGerilim.Text = "0";
            this.watSaat_dusukGerilim.UseWaitCursor = true;
            // 
            // wattSaat_yuksekGerilim
            // 
            this.wattSaat_yuksekGerilim.AutoSize = true;
            this.wattSaat_yuksekGerilim.BackColor = System.Drawing.SystemColors.Control;
            this.wattSaat_yuksekGerilim.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.wattSaat_yuksekGerilim.ForeColor = System.Drawing.Color.Black;
            this.wattSaat_yuksekGerilim.Location = new System.Drawing.Point(-133, 16);
            this.wattSaat_yuksekGerilim.Name = "wattSaat_yuksekGerilim";
            this.wattSaat_yuksekGerilim.Size = new System.Drawing.Size(26, 27);
            this.wattSaat_yuksekGerilim.TabIndex = 68;
            this.wattSaat_yuksekGerilim.Text = "0";
            this.wattSaat_yuksekGerilim.UseWaitCursor = true;
            // 
            // receiveMqtt
            // 
            this.receiveMqtt.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.receiveMqtt.Location = new System.Drawing.Point(1025, 358);
            this.receiveMqtt.Name = "receiveMqtt";
            this.receiveMqtt.Size = new System.Drawing.Size(117, 47);
            this.receiveMqtt.TabIndex = 113;
            this.receiveMqtt.Text = "ReceiveMqtt";
            this.receiveMqtt.UseVisualStyleBackColor = true;
            this.receiveMqtt.UseWaitCursor = true;
            // 
            // publishMqtt
            // 
            this.publishMqtt.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.publishMqtt.Location = new System.Drawing.Point(862, 358);
            this.publishMqtt.Name = "publishMqtt";
            this.publishMqtt.Size = new System.Drawing.Size(117, 47);
            this.publishMqtt.TabIndex = 112;
            this.publishMqtt.Text = "PublishMqtt";
            this.publishMqtt.UseVisualStyleBackColor = true;
            this.publishMqtt.UseWaitCursor = true;
            // 
            // temizle
            // 
            this.temizle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.temizle.Location = new System.Drawing.Point(39, 383);
            this.temizle.Name = "temizle";
            this.temizle.Size = new System.Drawing.Size(93, 47);
            this.temizle.TabIndex = 111;
            this.temizle.Text = "Temizle";
            this.temizle.UseVisualStyleBackColor = true;
            this.temizle.UseWaitCursor = true;
            // 
            // fan
            // 
            this.fan.AutoSize = true;
            this.fan.Font = new System.Drawing.Font("Arial monospaced for SAP", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.fan.Location = new System.Drawing.Point(1008, 270);
            this.fan.Name = "fan";
            this.fan.Size = new System.Drawing.Size(134, 17);
            this.fan.TabIndex = 110;
            this.fan.Text = "Röle-2 Aç-Kapa";
            this.fan.UseWaitCursor = true;
            // 
            // fanToggler
            // 
            this.fanToggler.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.fanToggler.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.fanToggler.CheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.fanToggler.CheckedState.InnerColor = System.Drawing.Color.White;
            this.fanToggler.Location = new System.Drawing.Point(1032, 300);
            this.fanToggler.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fanToggler.Name = "fanToggler";
            this.fanToggler.Size = new System.Drawing.Size(64, 27);
            this.fanToggler.TabIndex = 109;
            this.fanToggler.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.fanToggler.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.fanToggler.UncheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.fanToggler.UncheckedState.InnerColor = System.Drawing.Color.White;
            this.fanToggler.UseWaitCursor = true;
            // 
            // inputRegister
            // 
            this.inputRegister.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.inputRegister.Location = new System.Drawing.Point(150, 445);
            this.inputRegister.Name = "inputRegister";
            this.inputRegister.Size = new System.Drawing.Size(209, 53);
            this.inputRegister.TabIndex = 108;
            this.inputRegister.Text = "Input Registerlarýný Oku";
            this.inputRegister.UseVisualStyleBackColor = true;
            this.inputRegister.UseWaitCursor = true;
            // 
            // role
            // 
            this.role.AutoSize = true;
            this.role.Font = new System.Drawing.Font("Arial monospaced for SAP", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.role.Location = new System.Drawing.Point(862, 270);
            this.role.Name = "role";
            this.role.Size = new System.Drawing.Size(134, 17);
            this.role.TabIndex = 107;
            this.role.Text = "Röle-1 Aç-Kapa";
            this.role.UseWaitCursor = true;
            // 
            // guna2ToggleSwitch1
            // 
            this.guna2ToggleSwitch1.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.guna2ToggleSwitch1.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.guna2ToggleSwitch1.CheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.guna2ToggleSwitch1.CheckedState.InnerColor = System.Drawing.Color.White;
            this.guna2ToggleSwitch1.Location = new System.Drawing.Point(886, 300);
            this.guna2ToggleSwitch1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.guna2ToggleSwitch1.Name = "guna2ToggleSwitch1";
            this.guna2ToggleSwitch1.Size = new System.Drawing.Size(64, 27);
            this.guna2ToggleSwitch1.TabIndex = 106;
            this.guna2ToggleSwitch1.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.guna2ToggleSwitch1.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.guna2ToggleSwitch1.UncheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.guna2ToggleSwitch1.UncheckedState.InnerColor = System.Drawing.Color.White;
            this.guna2ToggleSwitch1.UseWaitCursor = true;
            // 
            // canliVeri
            // 
            this.canliVeri.AutoSize = true;
            this.canliVeri.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.canliVeri.CheckedState.BorderRadius = 0;
            this.canliVeri.CheckedState.BorderThickness = 0;
            this.canliVeri.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.canliVeri.Location = new System.Drawing.Point(886, 222);
            this.canliVeri.Name = "canliVeri";
            this.canliVeri.Size = new System.Drawing.Size(189, 24);
            this.canliVeri.TabIndex = 105;
            this.canliVeri.Text = "Canlý Olarak Veri Çekme";
            this.canliVeri.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.canliVeri.UncheckedState.BorderRadius = 0;
            this.canliVeri.UncheckedState.BorderThickness = 0;
            this.canliVeri.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.canliVeri.UseWaitCursor = true;
            // 
            // baglantiKes
            // 
            this.baglantiKes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.baglantiKes.Location = new System.Drawing.Point(862, 154);
            this.baglantiKes.Name = "baglantiKes";
            this.baglantiKes.Size = new System.Drawing.Size(117, 47);
            this.baglantiKes.TabIndex = 104;
            this.baglantiKes.Text = "Baðlantýyý Kes";
            this.baglantiKes.UseVisualStyleBackColor = true;
            this.baglantiKes.UseWaitCursor = true;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.button2.Location = new System.Drawing.Point(1008, 154);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(93, 47);
            this.button2.TabIndex = 103;
            this.button2.Text = "Baðlan";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.UseWaitCursor = true;
            // 
            // baudRate
            // 
            this.baudRate.BackColor = System.Drawing.Color.Transparent;
            this.baudRate.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.baudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.baudRate.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.baudRate.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.baudRate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.baudRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.baudRate.ItemHeight = 30;
            this.baudRate.Items.AddRange(new object[] {
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400"});
            this.baudRate.Location = new System.Drawing.Point(966, 94);
            this.baudRate.Name = "baudRate";
            this.baudRate.Size = new System.Drawing.Size(135, 36);
            this.baudRate.TabIndex = 102;
            this.baudRate.UseWaitCursor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial monospaced for SAP", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(862, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 17);
            this.label3.TabIndex = 101;
            this.label3.Text = "Baud Rate :";
            this.label3.UseWaitCursor = true;
            // 
            // portNum
            // 
            this.portNum.BackColor = System.Drawing.Color.Transparent;
            this.portNum.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.portNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.portNum.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.portNum.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.portNum.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.portNum.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.portNum.ItemHeight = 30;
            this.portNum.Location = new System.Drawing.Point(966, 46);
            this.portNum.Name = "portNum";
            this.portNum.Size = new System.Drawing.Size(135, 36);
            this.portNum.TabIndex = 100;
            this.portNum.UseWaitCursor = true;
            // 
            // portNo
            // 
            this.portNo.AutoSize = true;
            this.portNo.Font = new System.Drawing.Font("Arial monospaced for SAP", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.portNo.Location = new System.Drawing.Point(862, 56);
            this.portNo.Name = "portNo";
            this.portNo.Size = new System.Drawing.Size(98, 17);
            this.portNo.TabIndex = 99;
            this.portNo.Text = "Port Num :";
            this.portNo.UseWaitCursor = true;
            // 
            // gerilimProgressbar
            // 
            this.gerilimProgressbar.Controls.Add(this.label1);
            this.gerilimProgressbar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.gerilimProgressbar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gerilimProgressbar.ForeColor = System.Drawing.Color.White;
            this.gerilimProgressbar.Location = new System.Drawing.Point(706, 70);
            this.gerilimProgressbar.Maximum = 655;
            this.gerilimProgressbar.Minimum = 0;
            this.gerilimProgressbar.Name = "gerilimProgressbar";
            this.gerilimProgressbar.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.gerilimProgressbar.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.gerilimProgressbar.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.gerilimProgressbar.Size = new System.Drawing.Size(119, 119);
            this.gerilimProgressbar.TabIndex = 98;
            this.gerilimProgressbar.UseWaitCursor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(46, 47);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 27);
            this.label1.TabIndex = 1;
            this.label1.Text = "0";
            this.label1.UseWaitCursor = true;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Arial monospaced for SAP", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label19.Location = new System.Drawing.Point(709, 200);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(116, 17);
            this.label19.TabIndex = 94;
            this.label19.Text = "Power Factor";
            this.label19.UseWaitCursor = true;
            // 
            // gorunurgucProgressbar
            // 
            this.gorunurgucProgressbar.Controls.Add(this.label2);
            this.gorunurgucProgressbar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.gorunurgucProgressbar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gorunurgucProgressbar.ForeColor = System.Drawing.Color.White;
            this.gorunurgucProgressbar.Location = new System.Drawing.Point(706, 380);
            this.gorunurgucProgressbar.Maximum = 15258;
            this.gorunurgucProgressbar.Minimum = 0;
            this.gorunurgucProgressbar.Name = "gorunurgucProgressbar";
            this.gorunurgucProgressbar.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.gorunurgucProgressbar.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.gorunurgucProgressbar.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.gorunurgucProgressbar.Size = new System.Drawing.Size(119, 119);
            this.gorunurgucProgressbar.TabIndex = 97;
            this.gorunurgucProgressbar.UseWaitCursor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(46, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 27);
            this.label2.TabIndex = 7;
            this.label2.Text = "0";
            this.label2.UseWaitCursor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Arial monospaced for SAP", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label17.Location = new System.Drawing.Point(715, 358);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(107, 17);
            this.label17.TabIndex = 96;
            this.label17.Text = "Görünür Güç";
            this.label17.UseWaitCursor = true;
            // 
            // powerfactorProgressbar
            // 
            this.powerfactorProgressbar.Controls.Add(this.label4);
            this.powerfactorProgressbar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.powerfactorProgressbar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.powerfactorProgressbar.ForeColor = System.Drawing.Color.White;
            this.powerfactorProgressbar.Location = new System.Drawing.Point(706, 222);
            this.powerfactorProgressbar.Maximum = 15258;
            this.powerfactorProgressbar.Minimum = 0;
            this.powerfactorProgressbar.Name = "powerfactorProgressbar";
            this.powerfactorProgressbar.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.powerfactorProgressbar.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.powerfactorProgressbar.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.powerfactorProgressbar.Size = new System.Drawing.Size(119, 119);
            this.powerfactorProgressbar.TabIndex = 95;
            this.powerfactorProgressbar.UseWaitCursor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.label4.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(46, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 27);
            this.label4.TabIndex = 6;
            this.label4.Text = "0";
            this.label4.UseWaitCursor = true;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Arial monospaced for SAP", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label21.Location = new System.Drawing.Point(729, 46);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(71, 17);
            this.label21.TabIndex = 93;
            this.label21.Text = "Gerilim";
            this.label21.UseWaitCursor = true;
            // 
            // reaktifgucProgressbar
            // 
            this.reaktifgucProgressbar.Controls.Add(this.label5);
            this.reaktifgucProgressbar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.reaktifgucProgressbar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.reaktifgucProgressbar.ForeColor = System.Drawing.Color.White;
            this.reaktifgucProgressbar.Location = new System.Drawing.Point(554, 380);
            this.reaktifgucProgressbar.Maximum = 15258;
            this.reaktifgucProgressbar.Minimum = 0;
            this.reaktifgucProgressbar.Name = "reaktifgucProgressbar";
            this.reaktifgucProgressbar.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.reaktifgucProgressbar.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.reaktifgucProgressbar.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.reaktifgucProgressbar.Size = new System.Drawing.Size(119, 119);
            this.reaktifgucProgressbar.TabIndex = 92;
            this.reaktifgucProgressbar.UseWaitCursor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.SystemColors.Control;
            this.label5.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(45, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 27);
            this.label5.TabIndex = 6;
            this.label5.Text = "0";
            this.label5.UseWaitCursor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Arial monospaced for SAP", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label14.Location = new System.Drawing.Point(560, 358);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(107, 17);
            this.label14.TabIndex = 91;
            this.label14.Text = "Reaktif Güç";
            this.label14.UseWaitCursor = true;
            // 
            // aktifgucProgressbar
            // 
            this.aktifgucProgressbar.Controls.Add(this.label6);
            this.aktifgucProgressbar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.aktifgucProgressbar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.aktifgucProgressbar.ForeColor = System.Drawing.Color.White;
            this.aktifgucProgressbar.Location = new System.Drawing.Point(410, 380);
            this.aktifgucProgressbar.Maximum = 15258;
            this.aktifgucProgressbar.Minimum = 0;
            this.aktifgucProgressbar.Name = "aktifgucProgressbar";
            this.aktifgucProgressbar.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.aktifgucProgressbar.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.aktifgucProgressbar.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.aktifgucProgressbar.Size = new System.Drawing.Size(119, 119);
            this.aktifgucProgressbar.TabIndex = 90;
            this.aktifgucProgressbar.UseWaitCursor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.SystemColors.Control;
            this.label6.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(46, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 27);
            this.label6.TabIndex = 5;
            this.label6.Text = "0";
            this.label6.UseWaitCursor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Arial monospaced for SAP", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label15.Location = new System.Drawing.Point(425, 358);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(89, 17);
            this.label15.TabIndex = 89;
            this.label15.Text = "Aktif Güç";
            this.label15.UseWaitCursor = true;
            // 
            // frekansProgressbar
            // 
            this.frekansProgressbar.Controls.Add(this.label7);
            this.frekansProgressbar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.frekansProgressbar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.frekansProgressbar.ForeColor = System.Drawing.Color.White;
            this.frekansProgressbar.Location = new System.Drawing.Point(554, 222);
            this.frekansProgressbar.Maximum = 15258;
            this.frekansProgressbar.Minimum = 0;
            this.frekansProgressbar.Name = "frekansProgressbar";
            this.frekansProgressbar.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.frekansProgressbar.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.frekansProgressbar.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.frekansProgressbar.Size = new System.Drawing.Size(119, 119);
            this.frekansProgressbar.TabIndex = 88;
            this.frekansProgressbar.UseWaitCursor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.SystemColors.Control;
            this.label7.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(45, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(26, 27);
            this.label7.TabIndex = 5;
            this.label7.Text = "0";
            this.label7.UseWaitCursor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial monospaced for SAP", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label12.Location = new System.Drawing.Point(578, 200);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 17);
            this.label12.TabIndex = 87;
            this.label12.Text = "Frekans";
            this.label12.UseWaitCursor = true;
            // 
            // akimProgressbar
            // 
            this.akimProgressbar.Controls.Add(this.label8);
            this.akimProgressbar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.akimProgressbar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.akimProgressbar.ForeColor = System.Drawing.Color.White;
            this.akimProgressbar.Location = new System.Drawing.Point(410, 222);
            this.akimProgressbar.Maximum = 15258;
            this.akimProgressbar.Minimum = 0;
            this.akimProgressbar.Name = "akimProgressbar";
            this.akimProgressbar.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.akimProgressbar.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.akimProgressbar.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.akimProgressbar.Size = new System.Drawing.Size(119, 119);
            this.akimProgressbar.TabIndex = 86;
            this.akimProgressbar.UseWaitCursor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.SystemColors.Control;
            this.label8.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(46, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(26, 27);
            this.label8.TabIndex = 4;
            this.label8.Text = "0";
            this.label8.UseWaitCursor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial monospaced for SAP", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label13.Location = new System.Drawing.Point(445, 200);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(44, 17);
            this.label13.TabIndex = 85;
            this.label13.Text = "Akým";
            this.label13.UseWaitCursor = true;
            // 
            // watsaatAlcak
            // 
            this.watsaatAlcak.Controls.Add(this.label9);
            this.watsaatAlcak.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.watsaatAlcak.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.watsaatAlcak.ForeColor = System.Drawing.Color.White;
            this.watsaatAlcak.Location = new System.Drawing.Point(554, 70);
            this.watsaatAlcak.Maximum = 65535;
            this.watsaatAlcak.Minimum = 0;
            this.watsaatAlcak.Name = "watsaatAlcak";
            this.watsaatAlcak.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.watsaatAlcak.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.watsaatAlcak.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.watsaatAlcak.Size = new System.Drawing.Size(119, 119);
            this.watsaatAlcak.TabIndex = 84;
            this.watsaatAlcak.UseWaitCursor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.SystemColors.Control;
            this.label9.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(45, 47);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(26, 27);
            this.label9.TabIndex = 2;
            this.label9.Text = "0";
            this.label9.UseWaitCursor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial monospaced for SAP", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label11.Location = new System.Drawing.Point(555, 46);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(140, 17);
            this.label11.TabIndex = 83;
            this.label11.Text = "Watt/Saat Düþük";
            this.label11.UseWaitCursor = true;
            // 
            // watsaatYuksek
            // 
            this.watsaatYuksek.Controls.Add(this.label10);
            this.watsaatYuksek.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.watsaatYuksek.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.watsaatYuksek.ForeColor = System.Drawing.Color.White;
            this.watsaatYuksek.Location = new System.Drawing.Point(410, 70);
            this.watsaatYuksek.Maximum = 15258;
            this.watsaatYuksek.Minimum = 0;
            this.watsaatYuksek.Name = "watsaatYuksek";
            this.watsaatYuksek.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.watsaatYuksek.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.watsaatYuksek.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.watsaatYuksek.Size = new System.Drawing.Size(119, 119);
            this.watsaatYuksek.TabIndex = 82;
            this.watsaatYuksek.UseWaitCursor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.SystemColors.Control;
            this.label10.Font = new System.Drawing.Font("Stencil", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.Location = new System.Drawing.Point(46, 47);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(26, 27);
            this.label10.TabIndex = 3;
            this.label10.Text = "0";
            this.label10.UseWaitCursor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(52, 306);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(116, 20);
            this.label16.TabIndex = 81;
            this.label16.Text = "Modbus Adresi :";
            this.label16.UseWaitCursor = true;
            // 
            // modbusAdresi
            // 
            this.modbusAdresi.Location = new System.Drawing.Point(52, 334);
            this.modbusAdresi.Name = "modbusAdresi";
            this.modbusAdresi.Size = new System.Drawing.Size(308, 27);
            this.modbusAdresi.TabIndex = 80;
            this.modbusAdresi.UseWaitCursor = true;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Arial monospaced for SAP", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label18.Location = new System.Drawing.Point(397, 46);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(152, 17);
            this.label18.TabIndex = 79;
            this.label18.Text = "Watt/Saat Yüksek";
            this.label18.UseWaitCursor = true;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label20.Location = new System.Drawing.Point(52, 32);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(308, 35);
            this.label20.TabIndex = 78;
            this.label20.Text = "Modbus Register Adresleri";
            this.label20.UseWaitCursor = true;
            // 
            // textBox1
            // 
            this.textBox1.AllowDrop = true;
            this.textBox1.Location = new System.Drawing.Point(52, 70);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(308, 233);
            this.textBox1.TabIndex = 77;
            this.textBox1.UseWaitCursor = true;
            // 
            // holdingRegOku
            // 
            this.holdingRegOku.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.holdingRegOku.Location = new System.Drawing.Point(150, 380);
            this.holdingRegOku.Name = "holdingRegOku";
            this.holdingRegOku.Size = new System.Drawing.Size(209, 53);
            this.holdingRegOku.TabIndex = 76;
            this.holdingRegOku.Text = "Holding Registerlarýný Oku";
            this.holdingRegOku.UseVisualStyleBackColor = true;
            this.holdingRegOku.UseWaitCursor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1171, 539);
            this.Controls.Add(this.receiveMqtt);
            this.Controls.Add(this.publishMqtt);
            this.Controls.Add(this.temizle);
            this.Controls.Add(this.fan);
            this.Controls.Add(this.fanToggler);
            this.Controls.Add(this.inputRegister);
            this.Controls.Add(this.role);
            this.Controls.Add(this.guna2ToggleSwitch1);
            this.Controls.Add(this.canliVeri);
            this.Controls.Add(this.baglantiKes);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.baudRate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.portNum);
            this.Controls.Add(this.portNo);
            this.Controls.Add(this.gerilimProgressbar);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.gorunurgucProgressbar);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.powerfactorProgressbar);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.reaktifgucProgressbar);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.aktifgucProgressbar);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.frekansProgressbar);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.akimProgressbar);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.watsaatAlcak);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.watsaatYuksek);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.modbusAdresi);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.holdingRegOku);
            this.Controls.Add(this.gerilim);
            this.Controls.Add(this.gorunurGuc);
            this.Controls.Add(this.powerFactor);
            this.Controls.Add(this.reaktifGuc);
            this.Controls.Add(this.aktifGuc);
            this.Controls.Add(this.frekans);
            this.Controls.Add(this.akim);
            this.Controls.Add(this.watSaat_dusukGerilim);
            this.Controls.Add(this.wattSaat_yuksekGerilim);
            this.Name = "Form1";
            this.Text = "Rtu-Modbus Haberleþmesi";
            this.gerilimProgressbar.ResumeLayout(false);
            this.gerilimProgressbar.PerformLayout();
            this.gorunurgucProgressbar.ResumeLayout(false);
            this.gorunurgucProgressbar.PerformLayout();
            this.powerfactorProgressbar.ResumeLayout(false);
            this.powerfactorProgressbar.PerformLayout();
            this.reaktifgucProgressbar.ResumeLayout(false);
            this.reaktifgucProgressbar.PerformLayout();
            this.aktifgucProgressbar.ResumeLayout(false);
            this.aktifgucProgressbar.PerformLayout();
            this.frekansProgressbar.ResumeLayout(false);
            this.frekansProgressbar.PerformLayout();
            this.akimProgressbar.ResumeLayout(false);
            this.akimProgressbar.PerformLayout();
            this.watsaatAlcak.ResumeLayout(false);
            this.watsaatAlcak.PerformLayout();
            this.watsaatYuksek.ResumeLayout(false);
            this.watsaatYuksek.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Button receiveMqtt;
        private Button publishMqtt;
        private Button temizle;
        private Label fan;
        private Guna.UI2.WinForms.Guna2ToggleSwitch fanToggler;
        private Button inputRegister;
        private Label role;
        private Guna.UI2.WinForms.Guna2ToggleSwitch guna2ToggleSwitch1;
        private Guna.UI2.WinForms.Guna2CheckBox canliVeri;
        private Button baglantiKes;
        private Button button2;
        private Guna.UI2.WinForms.Guna2ComboBox baudRate;
        private Label label3;
        private Guna.UI2.WinForms.Guna2ComboBox portNum;
        private Label portNo;
        private Guna.UI2.WinForms.Guna2CircleProgressBar gerilimProgressbar;
        private Label label1;
        private Label label19;
        private Guna.UI2.WinForms.Guna2CircleProgressBar gorunurgucProgressbar;
        private Label label2;
        private Label label17;
        private Guna.UI2.WinForms.Guna2CircleProgressBar powerfactorProgressbar;
        private Label label4;
        private Label label21;
        private Guna.UI2.WinForms.Guna2CircleProgressBar reaktifgucProgressbar;
        private Label label5;
        private Label label14;
        private Guna.UI2.WinForms.Guna2CircleProgressBar aktifgucProgressbar;
        private Label label6;
        private Label label15;
        private Guna.UI2.WinForms.Guna2CircleProgressBar frekansProgressbar;
        private Label label7;
        private Label label12;
        private Guna.UI2.WinForms.Guna2CircleProgressBar akimProgressbar;
        private Label label8;
        private Label label13;
        private Guna.UI2.WinForms.Guna2CircleProgressBar watsaatAlcak;
        private Label label9;
        private Label label11;
        private Guna.UI2.WinForms.Guna2CircleProgressBar watsaatYuksek;
        private Label label10;
        private Label label16;
        private TextBox modbusAdresi;
        private Label label18;
        private Label label20;
        private TextBox textBox1;
        private Button holdingRegOku;
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.IContainer components;
    }
}