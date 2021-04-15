using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace WebApplication1.Utility
{

    //Symmetric Encryption/Decryption //use this one to enc/dec the querystring values
    //Same key to encryption and decrypt
    //Adv: Fastest, Simpler
    //it uses Secret Key and IV + salt
    //Disadv: you have to be very careful if you are going to use this one when sending files over an insecure network

    //Asymmetric Encryption/Decryption
    //It uses two different keys: Public Key and Private key
    //Public Key: encrypt
    //Private Key: decrypt (its always recommended never to send over an insecure network the private key)
    //Adv: we now can transfer the keys securely over an insecure network
    //Disadv: this is a slow performing algorithm, you cannot encrypt/decrypt large data. normally used to encrypt base64 data


    //Hybrid Encryption/Decryption //this is need to encrypt the files uploaded by the students & decrypt the files when they are needed by the teacher
    //we r going to take the adv of both the symmetric and the asymmetric to form the hybrid
    //we are going to send over the insecure network the encrypted data which was encrypted symmetrically and therefore we are going to send the
    //encrypted symmetric keys (which were encrypted with the public key)


    //Hashing
    //1 way encryption
    //usage: in passwords
    //sha512
    //two different values should give two different digests e.g. "hello world" and "helloworld" >> 2 different digests


    //Digital Signing/verify
    //against repudiation (when an attacker tampers with the data and he denies the activity)
    //suggestion for the assignment: you generate a private & public key for every student
    //when the user uploads the file, you take the private key and you sign the file and you store the signature with the file's data
    //when the user/teacher needs to download/view the file we need to use the file's owner public key + signature, to verify whether the data is still the same 
    //   i.e belonging to that user




    public class Encryption
    {

        /// <summary>
        /// This method takes a string and returns the digest as a string
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns></returns>
        public static string Hash(string clearText)
        {
            //convert the clearText into an array of bytes
            //clearTexxt is a non base64 data it has to be converted this way:
            byte[] clearTextAsBytes = Encoding.UTF32.GetBytes(clearText);
            byte[] digest = Hash(clearTextAsBytes);
            //base64 data convert to/from a string we have to use this class:
            string digestAsString = Convert.ToBase64String(digest);

            return digestAsString;
        }

        public static byte[] Hash(byte[] clearTextBytes)
        {
            SHA512 myAlg = SHA512.Create();
            byte[] digest = myAlg.ComputeHash(clearTextBytes);
            return digest;
        }


        string password = "Pa$$w0rd";
        byte[] salt = new byte[]
        {
            20, 1, 34,56,78,34,11,111,234,43,180,139,127,34,52,45,255,253,1
        };


        /// <summary>
        /// this method is used to take in clear data and encrypt it and returns back the cipher (the encrypted data)
        /// </summary>
        /// <param name="clearData"></param>
        /// <returns></returns>
        public byte[] SymmetricEncrypt(byte[] clearData)
        {
            //Note:
            //1st thing is to think of how you are going to handle the keys
            //solution a) the key can be hardcoded
            //solution b) the key can be randomized/generated out of something the user's password
            //solution c) a password and salt which are hardcoded (which never change) out of which you generate "randomly" the key
            //in the assignment:
            // 1. to encrypt/decrypt query string values
            // 2. to encrypt/decrypt the file data as part of the hybrid encryption/decryption

            //password will be the source of origin of our secret key
            //the salt will add more security against an attacker guessing the password using dictionary attacks

            //how do we generate a secret key
            //note: each algorithm has a different sized secret key + iv 

            //0. declare the algorithm to use
            Rijndael myAlg = Rijndael.Create();
            //1. first we generate the secret key and iv
            var keys = GenerateKeys();

            //2. load the data into a MemoryStream
            MemoryStream msIn = new MemoryStream(clearData);
            msIn.Position = 0; //making sure that the pointer of the byte to read next is at the beginning so we encrypt everything

            //3. declare where to store the encrypted data
            MemoryStream msOut = new MemoryStream();

            //4. declaring a Stream which handles data encryption
            CryptoStream cs = new CryptoStream(msOut, //target stream where to write the data
                myAlg.CreateEncryptor(keys.SecretKey, keys.Iv), //the engine that operate the encrypting medium
                 CryptoStreamMode.Write //this will write the data fed into the medium
                    );

            //5. we start the encrypting engine
            msIn.CopyTo(cs);

            //6. make sure that the data is all written (flushed) into msOut
            cs.FlushFinalBlock();

            //7. 
            cs.Close();

            //8.
            return msOut.ToArray();

        }

        public SymmetricKeys GenerateKeys()
        {
            // Password + Salt >>>>> Algorithm >>> Secret Key + IV (which is the input needed by the encryption algorithm)

            Rijndael myAlg = Rijndael.Create();

            Rfc2898DeriveBytes myGenerator = new Rfc2898DeriveBytes(password, salt);

            SymmetricKeys keys = new SymmetricKeys()
            { //1 byte =  8 bits
                SecretKey = myGenerator.GetBytes(myAlg.KeySize / 8),
                Iv = myGenerator.GetBytes(myAlg.BlockSize / 8)
            };

            return keys;
        }



        //public string SymmetricEncrypt(string clearData)
        //{
        
        //}



    }


    public class SymmetricKeys
    {
        public byte[] SecretKey { get; set; }
        public byte[] Iv { get; set; }
    }
}
