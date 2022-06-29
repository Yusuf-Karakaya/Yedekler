using System;
using EasyModbus;

namespace proje1
{
    
    public partial class Form1 : Form
    {
	
		public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
			
		}

        private void button1_Click(object sender, EventArgs e)
        {
			ModbusClient modbusClient = new ModbusClient("COM3");
			//modbusClient.UnitIdentifier = 1; Not necessary since default slaveID = 1;
			//modbusClient.Baudrate = 9600;	// Not necessary since default baudrate = 9600
			//modbusClient.Parity = System.IO.Ports.Parity.None;
			//modbusClient.StopBits = System.IO.Ports.StopBits.Two;
			//modbusClient.ConnectionTimeout = 500;			
			modbusClient.Connect();
			textBox1.Text += ("Value of Discr. Input #1: " + modbusClient.ReadDiscreteInputs(0, 1)[0].ToString());
			//Reads Discrete Input #1
			textBox1.Text += ("Value of Input Reg. #10: " + modbusClient.ReadInputRegisters(9, 1)[0].ToString());   //Reads Inp. Reg. #10
	

			MessageBox.Show("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
    }
}