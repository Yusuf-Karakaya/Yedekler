
using NModbus;
using System.IO.Ports;

SerialPort port = new SerialPort("COM3");

// configure serial port
port.BaudRate = 9600;
port.DataBits = 8;
port.Parity = Parity.None;
port.StopBits = StopBits.One;
port.Open();

// create modbus master
IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);

byte slaveId = 1;
ushort startAddress = 1;
ushort numRegisters = 5;

// read five registers
ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, numRegisters);

for (int i = 0; i < numRegisters; i++)
    Console.WriteLine("Register {0}={1}", startAddress + i, registers[i]);

byte slaveId = 1;
ushort startAddress = 1;

// write three coils
master.WriteMultipleCoils(slaveId, startAddress, new bool[] { true, false, true });