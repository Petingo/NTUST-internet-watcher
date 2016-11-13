using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Web;
using System.IO;
using System.Threading;
using Microsoft.Win32;

namespace Internet_watcher
{
    public partial class Form1 : Form
    {
        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        public NotifyIcon notifyIcon = new NotifyIcon();
        public string download , upload , total;
        public string currentTime;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            getData();
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            myTimer.Tick += new EventHandler(getDataAndInitial);
            // Sets the timer interval to 3 min.
            myTimer.Interval = 5*60*1000;
            myTimer.Start();
        }
        protected override void OnResize(EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Initial();
                //通知欄顯示Icon
                if (notifyIcon != null)
                {
                    this.ShowInTaskbar = false;
                    //隱藏程式本身的視窗
                    //this.Visible = false;
                    this.Hide();
                    notifyIcon.Visible = true;
                    notifyIcon.ShowBalloonTip(1000, "監控IP", textBoxIP.Text , ToolTipIcon.Info);
                }
            }
        }
        private void OnApplicationExit(object sender, EventArgs e)
        {
            if (notifyIcon != null)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            }
        }
        
        private void Initial()
        {
            getData();
            currentTime = System.DateTime.Now.ToString();
            //設定通知欄提示的文字
            //notifyIcon.BalloonTipText = "Still running";
            //設定通知欄在滑鼠移至Icon上的要顯示的文字
            notifyIcon.Text = total;
            //notifyIcon.Text = currentTime;
            //決定一個Logo
            notifyIcon.Icon = (System.Drawing.Icon)(Properties.Resources.NTUST_LOGO);
            //設定按下Icon發生的事件
            notifyIcon.Click += (sender, e) => {
                //取消再通知欄顯示Icon
                notifyIcon.Visible = false;
                //顯示在工具列
                this.ShowInTaskbar = true;
                //顯示程式的視窗
                this.Show();
                this.WindowState = FormWindowState.Normal;
            };
            
            //設定右鍵選單
            //宣告一個選單的容器
            ContextMenu contextMenu = new ContextMenu();
            //宣告選單項目
            MenuItem notifyIconMenuItem1 = new MenuItem();
            //可以設定是否可勾選
            //notifyIconMenuItem1.Checked = true;
            //在NotifyIcon中的頁籤，順序用
            notifyIconMenuItem1.Index = 1;
            //設定顯示的文字，後面的(S&)代表使用者按S鍵也可以觸發Click事件!
            notifyIconMenuItem1.Text = "關閉";
            //設定按下後的事情
            notifyIconMenuItem1.Click += (sender, e) => {
                Application.Exit();
            };
            //將MenuItem加入到ContextMenu容器中!
            contextMenu.MenuItems.Add(notifyIconMenuItem1);
            //設定notifyIcon的選單內容等於剛剛宣告的選單容器ContextMen;
            notifyIcon.ContextMenu = contextMenu;

        }
        private void getDataAndInitial(object sender, EventArgs e)
        {
            getData();
            if (notifyIcon != null) {
                notifyIcon.Text = total;
            }
        }
        private void getData()
        {
            try
            {
                CookieContainer cookieContainer = new CookieContainer();

                ///////////////////////////////////////////////////
                // 資料來源 = http://www.cnblogs.com/anjou/archive/2006/12/25/602943.html
                ///////////////////////////////////////////////////                
                // 设置打开页面的参数
                string URI = "http://network.ntust.edu.tw/";

                System.DateTime currentTime = new System.DateTime();
                currentTime = System.DateTime.Now;
                string postString = "__EVENTTARGET=&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE=uF8e%2B0cBNQkHTkgXvDKWQ1fj7oaJmDltzoxyik6zadCWQoPi5TOSu5zhu%2BGxVSm1UwQfbeYuJp6SjlUCYadZjNBbBX%2BP1uGT1g0uDfLZwZNDDJyq5mFZbB8cU3P08iqdzHafgmMcUzbwha%2FAyJUv0v2zwi0wwcCbKgVjQnoEEAsYZXBSFtsX8GYjmN5SJIbNBjHecH3CUwZoFgWkAOm684nATnRXk6aWjNPZJNkXVUbZEvHekm6xM4Bphd1LlTwGJOim5enZjh8jQPZUPE%2Fh4Aokql%2F5NgCUYBTTRXGOsvVr6dtjRMwJdYrt9387r0FnkjonhKtsULnHD62UAW5WVD5VdEXh6Bl5YCbHBlkNgWJIl%2BIDNQ08VdceysYUBKRhil4Ju12mPWfzlExytrRQelg378zW6AjnVPZ7o2hFzfRt5byrdMZCdI7XlvZZk%2F%2BKsw08McqbOGlVjINje4yFfSQRKux84boLc8zRu7q4HptGlwmAlKhEpqlRbeJJPzdFTjzwKhaACR3%2Bnk6PbQv8eooZIMPh%2FlQ2Ac04i9d7HoY8ecnJn0oedZOadY97loUkmUv2RDCPLO0zOighYTReApdD32JoIvfSubwS25by0EL3sOBWaxGudEdzmLYMHjM9tVwS37KId2cwsym7MuSj0KljfoYbmnIVIenczjSWRbjHbo8yRD2ElTMTYlK1z7YVffBf2rgGidLgu2Im5Y9HCnLkueT10e6SBwjSezrqgVYdrbiIYnn%2FrsWFhX4Afn5oxzLZi9nGzxr22rETKW4jbdbxZo8PFPfTr8bDogCI2Y97SJ8TOfHuAYAYXoKPuYzDIAv67%2FrKJ1KRjSxaufU5NfM0BTPQahiMz%2F9fp9dBETBTg1u4uVoFFcmzvYZ7XuZe%2BLrUYbR2j0QaHxISfL09GXna4G7qXf5ei1W9cLwEr5h78NJRX7JTzSYkt%2B0dmw%2FVOSv1ogDQycWc8YncPbLku0sB9aLNnUWRkxs5WsZKPiQZvVsgjVXzYirvSK1TfvVyvyqCs0rXTvSVdPc4yf7YqaUQ8mpFsFowlBlLjWvP%2FmEisuQZqV01tI0s34VJZ8FE4IU82E7UpdwGqUI5vuqWn5GILlXEM%2F%2BfbhBY8%2BWfhwxArmXfk0QNwtvr0PPGoTVGOXyoKfWDJss8XFmnTVF5y90GxE7rKoo4A1%2FlOdW0sy%2B5abxFFdKnjhM7lIR%2FLLAtNWCjd90olpqFUb19%2Bh2Ow8jDzbP75CJWybYmBhuQtFJNtvMlhsGhZ4A%2BJrbl%2FX7FNEHWwg5bGZvKvG1znfUNABL3gNi0cw27zlcl%2BDBEPmIMS6tz2uY5ksbhnigpO1LssmY80IKTR7a0JrlFgM2ubjimVXxJrCm4IbwIGm8f84EOo9ATHMKL9kdq4NyroBkJKwZUzUMQoXYM%2B9IpWs7keIRadco3gWMTU4AQHhqzK8YdPVzVnVmQaQHJOKMeyqUq6riRRucDRJRxn%2BYdzi5KkX%2FB%2F2qeK16%2BRyB9ExV5%2F1dqRnlKZFB00OvqnqK3q5b2JlK8CylkBB2Akxc%2FToaN3SVyvgfq39jrO%2BEw8YkcYkz4qnTPJWq1woXZR%2BdKTyDtNd8Yns%2BCYajSJL1KmrlE3%2BCkvgRsjC6xL5sEg4RwUkXk2Yx7CYptD5os0CtNeAl64IlEVw6JvvjL%2FodkkTTT%2Fhwml3N1JnA2yD37ousblJDmoo7S2%2BNDID4eTBH5idBT%2F1aKH%2B1OzmFYDwRP1maxcN8LfdvsU0OVnjWjciNi2UAlOXJYb3JyKpTFNuEOXlD%2Bdtl6OSp4Q4hL2ff5Dangp3PPe7rmFDUss8d0PrkRyH3DShUzSkqZIJXffbY%2BHTggnKFIjzap7Axls0hjMjN4miPFEfHTrFXrnQqyqD3mVhCViukauvQed40c7CNTEn6UXRXqNA7cfTa0hR6mEWjfZJX45f1XSP6WeUN0EsNMrnEUCunZyfKoQ5%2FPe2t1Hzj5GmgM9V84cDOXM9i6dBJC0eTRtjj%2BkvrdgS%2Fmg8JyvptiOfko6o%2F1S4JSZPYVlJylLEQOdc885U9xBxdYRvHfQ%2B3z2r3gK2n5GG73yLduZQ8U2mOrh5N2L92lrtrqxhgZ88CwAOqTisxxQrXT1udiPtdmgTbn3wkM80YK1uF7n8%2FFhNjo2HCyIKA5Kb4X40VJzLGsT7oFu7rFYLLDdDCJNtadP8YTPMbjbTQH36aWxh64NxcaMFqIjA%2BNj8809QS1XOOO1NXvGxQevc6FsOmqyZrKnCsungcfajkDW6Z2KHuErwyuDzm5TbghNBUq3YPs8qm8WNMfWyQ8Yq%2FGSG5mSIvf1yMnFI6WRAsaqHtjbxfrSt1FF5lOwsoZMrrRVM63RRJW39O3evjdIruJ4co3JcqfgnGoI5%2F7achOq%2FMcqplIWiZvAP%2BErvVgO%2B5JwgmvhaCco4fvjBbWjS3CX9HDz3C%2B%2Fik4GGI7e0cgXE4t3RiSGPGmhIwxN7KXxWzuBVODlZ0iI%2BXaJPZr0jkvVjiLTxhoPT4YlxZjIkRxO%2Fvd88A0TLE3pH0%2F6G44ztJ6XK%2FZCAlgfipW9TqDCcQvefeoHzYmk8Xq4zZEnKpHYFs48by0lWlg%2FarOevv1GBrlECM2pOkW2DdAHHKvgwhwVAPH8WzFQLxbnuWtiCVwRDWuyiMFk2KydNuKphyXUXSk6TwMNwS%2F7UDRujbSVbOPd%2BNCBNPNbUFB8YpG3BsxzZlr4ed%2Bt5D9Y6o6ZIhCO5rKUrwrYem1FmJlQVBRD2QFADLpjlLOL2IfHWgIZGVYxDYA2lfCvp47UkWtw7YQFqKskngvDHbSgpmBBFvI%2Bx1gPEZa%2BrmikNgFUiP6P4Y53CY%2BqOno3JqrlJSz20Z%2BpsbdNoRmVgzi7f071Ub4Xs6BFYF2RQMrQQW%2Fgz4nOuFKAJAAEMTl%2BXa6J46hl%2BAjpOnOs2Kp2N6R%2Fg9uLHxldqNvbSmgTMBK1tJe%2FXRFhpcH605bmA4owxp6HqfPMTgocnSih%2BNK2Pgt7uf1809Y5YJJXq1QXyr1iWtHcCJf5vIZF5DzNua%2BY%2F5maj2SCEleKs%2BKLPyfwftzpw3ctTOZEH0ysjOAfHij36x%2BrOd0imWa5GMaTwsXusaXEB5MFHQkz1Nfyx4L9kfburSK34q8diZ1O5t2iACCQ3b6Lrz915SEXfNyY7FL6qd3hkPt4sV0FzBONOi4SQ7nRndKB98zL70Jr50IZBixGlZEG8OTGyNGikkmQON8QbIN8%2BADH8T0fSJaZiqr9zmVERWol2tZXxb5HGIF3pnJaTOo9I5nJVYNsEvCZ9HDFWp4Bo6P%2BucRcOfo07IV9EaJD%2BfTUlHy4rJQtRxblmDwj79uEVgJyHcDG4nHhhq3sCqVTV0V5aS1Yw%2B47qh%2FhqTcQeol4jchOK4aCZGeW%2BI0GaeJ87x0IadsgoI07gTZ6453vFdH%2Fd5%2FPDqjfnEuczckt0JVxQoOzv3KeArF7IPCSBoHWxcs2mfGB6bv%2BK8K404o85nswCpPJCqhg%2FuiMH81dUoCaHiWhWRCYsOHeKf9oQrTSgNElYY20WAEDoObjp4nHT19uC%2BefThUl30BtkGEWZRt8%2FbyIYkwyYAr88WCSH7pA2xxbtiMJ6CTpjg1o2u7RsPhN6v914fEflim4EOqVWlbo41OunXcgLOGGUBWNt6rGFJHR3mx6Fw5xu6%2BASQvF2j3ST4smStypwsnsZk3v0w72fvfHCvciPi9i%2Btekjrtle2sEjds4A0F8qrs5UvOwvbvGGLFJq%2FvfKA8F3V%2F0gOM2%2F0GrMyx4gxlD%2B4mvrORLqSRCdC%2BBUZUHtqTp7R2Tz0SwkJBSa7IMtN6VvG7sjb69EcZa7lIgTNbB1BPOFvhCwlOr5B%2BbjDRG%2FLdupjEzgmJkFmd3YNkrBNV4VVhX%2BV1DwIsyqaI3bScg9D1o4u3ok1fZnCtjLUwbjbDAh7SLIOqLUy9EGe9MilD%2BueDUtchW9AVZWeuv7gZ4r03lRFELXS2c8N6UYdEC7DR82lVqSmlZHAktsMH8K2N9bAHvZy%2F%2FtnF9pBZ1KfbR7%2B51IIOzWEfLjxGMYAKC6XfhvlkV93gG%2BNbUM5nXNqbipXpi7fIRTEVOOkYRdqhp1Gl%2F%2BGTTaX6gN%2F8BAZWa0tbJBslbz%2FiYcQb7vXMsmfXA3tXzxv1qSAGKmE6xVTRX6AyZW9QNh1vs8ijjqARQ3HDZmrd%2FEfPXCvxRwktZwRf5Wv7KUQv87FjbNUXIj14siH0wQfoFuHWSef%2BsyAbvRNHwv0ngvQG5YG3Qw8%2F23iYfRo3LhnPDybA43y7v0El3eUEcaJu3CK8MYxMZYC%2BI3jO7ZSLiHM9JPSRfixhSBAfVgF%2FomXpuRvjcmNHziqEfzT5los7z8AVWKTrRYKuVDH4O0wsJNFW2GuC2jYWoL8ssCQCVw7WQztN0Ek%2FUB3HjcCIpgqAfS2oI1hWJrvV9rkjX5ODCkBRSUq7JtTBP8zZP8u8cBQr3daOjoZrssl4RynVUT%2BpuJjCAMsRO7al4sRokfZuzVzqlYpqKU7VtE1XunqpSu3TpwOeFvmUzJ6noX7FnAIluedefi3XNQD3EZ%2Bb21XcCfbGjM7%2FleqT%2FaxH4ArIchev%2F%2F%2B967MNgoyAUlOS8G485dtB4%2FHLIZyPrqnCHIesFeNzNMmkF%2FvcVKTDBUgGEsniVFD7oKauivyTQec23%2BHBmaeNO1eeufQSp5WVyyTQwG5Iu5sFtEooaS3IVrIAAXyTIxMqm9QzkkrNTcQNFPOtnuaht8YyVcXXBhVgfLQYQlF0CKMvivV0vTbEyyxw0V%2BTue1CZlh5Tkkmq7A8PK64BMTufEzjEl0V3QkrlDUcf5EQ2kEFZJY%2FVenf7pSijg%2BBC0lKmTZhq8kJDCDppTftmmvF9Q6eZX9TNsM27AKGb2vx5GTPkvIj595caxTjjTza0Z6moMJAcsFB7W8hHi7Ay5f2dn3BFecNzQdFWhyHuqu2DM9fNTQG5a2%2BET%2FYw%2B0BYduxIukEA0ePULEHiK38XutClZeq0dffJXrXYzoS3Hf1VYuHHVgCHQbLAFtljHOVV21QRSjkpVnI7ETH8g8DriQnp47Uimew6gUpOZhowBXhDyYRHjHAEJcmkZqK5FwWUCQN09J8WXWwj8%2FeJMZdTGFPeRkFDl31FaI30zhrcQVPDgeSaQ7dy2bVkkaKB%2B0zV4VM9d9zMKuK0p46lPpXLqTUiDkpMbrxvaA5%2FtRMDum%2BwBU4snRvm43ka51vb%2FBqVmaSM3oBdfUtVb4jp88VV6O59ogDyWF1O4HeJLhYo%2BJh%2FDWUK%2FgShfhFqUw%2FHF1m8T4IyOfRMCGFx6YhIfs9QCnNSCgtPi%2FlXir45sZP285%2FxB%2Bv0mzHrqAYkCIsFsSfBXvfQnARR4XT4rFb07mvjfg3MSJieOZxw%2BegSw8edINKh63hecQQmmqDLllyTICHs9c%2F7pNVAJ1QV7sTCRvYdSLFyQ51hLwH9rWlVf2zIcUpRFPtq0RTkRbHkA%2BPyLa8VmIcffEOSvhhXjZJ6MT8dPcnBGDNVWiCP2hvoe9cI0ROqKZPnhREltgh7iYC6G%2B8IZotBjkZ3sZjA67rMfKl9ycGzJNfZgi%2BBpASDOf%2Fxt3B3h4kzO5FEtizO9d%2BMDcAWuBY78mjMrHtWb4IA82gmT%2BkT%2BoSpjsP1%2Fj1qaT%2Bq%2FQ5L1Y4%2FQojuV95fnH6sT5%2F3f5MKpakDoAq8SP4DYYjmHC%2BTY7ItMN4yTSVnwWNT7v6Ihbwg%2FyCdr1H1Nugnq%2Fhu2TZf5kiixdnxfgDi1FMIs%2BgIxF5DRUZAz%2BSn35Rl73g3KawoTyyJtNvE5vyWuF3b%2Frm5H44XwmtMqbwawpyX1GTnb1pR%2FQXHgMZwzz1lDLMH50zgEovec6motlxn9nCvQycQyCHmYzc7HKPvrolNR%2FpROKi5HEs3R%2FYkufyf9uMPAtlJKCwB2wGPQOqpY0NBRrxSNKPcdyWeWCuXeVLFy00u1HM77u4VzXOPQJVnKCb5ue8kTiIXjQ7SS8uSRNaynmy8%2B9VC8xuPjGW1LYFOqepWeBkgb7Dl%2FBZpeTqy%2FcRWZ1b32dvltrlh5PpswwOjEQtFpoi%2Buz6rd8RE4t5ZPvXGJWQmNJCMHstM42%2B3v5NB431P4CWnHPh%2FbBMBEswaaagbwLvoqEroudi0EEivx3wTQFqGyih%2FWhq%2F91YQD3pnsUc6Q%2FgDaHB015aGYSX0F7IQqyNeTnF9luKBJOTfDBDDCC2BE4Fl1G7%2B0ha5kI2RcmRXBMk%2Bd18iJFzGPNBbDTBzdUSijDQ3J4NKHPWWI%2Brc65iNQYtLAvfpGZuZhnIVVjTlOVy9TobYTyMLffhnF%2B6UMf1OpbDKarg%2F6YMEUEDQHNb1zwYo7780r9%2FVyV8az%2BJSO4j8zadsxLcaiDAlNqcUrRZCDgNp0XioFubkH9oRCmO%2FH3sbwE62CpVmp0SrsuhOSwLBP4Ilpn5QhdhwTfbVk5bS%2Fv6ZMHoc6go%2FzmtHGpAB3ZBpy%2FPStoGc1huIqMhzc%2FZu%2Bn4sPt6QSmXEwLuYehcNpEsouJCkIOFdA2A0vsiU1OWgjkv61i5JGjrtywZHyCrfIRytoVjhAHschrprDGs4oEMg9LdqhC5qfbD%2FHkQ%2BsvJayh%2FEOb271CGZ7p9metHIzGKkYIMypWKwTptk0OZrUQpFhbP0s4JOCM3MyPGs3Co5MRpHeX1MTMw6t%2Ff1oe9dYcNBEqdGAYQN%2BCQjlL%2BpH%2BiAEhZvG0frvcAI6Q4K1QhAQSGS8IBGS4hKQUhrXYvdQhX2opxAOTLapPVwQggJd7jCukDNJtDXu%2BdmObFBdG0kLEb8ELPuEnVaO6aFkDaezKLxDZ1ma2MnypsQvD7n6U1cDH9%2FkBg56oYBWPQUI%2BacDrsHHCpiDDwy%2BcjU0bpE1aXgK48TRz4tsp05aymeThsF96FlwlKW2BrG5w9kzMvfOgKMQsZNvcDnlmc5pAYVtdex5oOSsMBxuqTQ9PiZ3c%2F9%2B0a%2FzdyykzTAGj4G%2FAEMQdDPOXp55AxeRLdq9XyPqSHfwGVAqMgx4VMTZCqTi1zyB6Ahl5BpyzazfwjgYL35lhRL0mOZsQdo%2B0tAn2yhPK%2FSGncEaYYt2PnfO8pgYC%2B%2BWlsCWDeLsStc843Uric7w5%2FV0I7ga7akTLNAYe8yPzCRRAekvz1%2Fq03PwGfOE32S4B%2FMVn235%2ByIMUaJcI%2FzD69Vcjw4%2BeAavbDunpU%2B4KOYxzs%2FZuQFroyR2f4rPbKsBO8ybJGJAIwQzDduoVPfFEkauDXqM2RA3BaivANA2MG1iAvj0DYoosvqFwDHvK8m0MZTAsbOTHpF8WrejbLZxSFRIH%2F1KbGyq%2BGY5iu9AoccAY%2B%2FdybkQYkSZb8xRPqgti8fnbJM%2BLsfbE7veGtOlnyVsCKsi1DgYrXoZvG8%2BXmgIj9a77kZ7NLRh68Tl2PYD3lnOP8jQwq0RIDHvQtPDQiY72di6tcvdKhoPxy8BLXwbcYNgbFGJCF3r3uiWmH5E3FlcZ%2B9vdmL2PBiH5Eggv7heukipohjkA1FiUWnch4BHyB3Sve6eHxu5wDSVSJmb0pCbkfibLOxJUl58TghN8aFHZCHCzPbZ%2BuauGegGHYVuD2BJBtXSxZ%2BBsZevnO6HICLf%2BnkYuDlJqBUPMaN7fGGrerzIRGd3kiUOgemq37oEev5ELfhuGzX5mwwE56rfSBaqnnYjBGOseE6Ji7KEojnNzXldTiPHJHfWmdY9kFOmrUkSYisVMJZgjT5VaYuEjW2Wtrs5OiSUFPKtw1Pf6mMqihxgdSNBx9ZGCt8%2B7llZuYa4cOgQ4yro3UE0K%2FHZ1w2ZL0pPCcKpOQryYzPkyazotuvA4O8bnaWURIgYq4ZX43JCfu8yO5IpO8fO%2Beb6eJpW%2F7jCOuFB9mfEDQQRxU4%2FgZJWXfVY%2F31vi2Fq1C6Wgkdqj4gkFg5hRIYRy5tQ7XK9Q2%2FjoYl6gqkKLD7IROpxkQ2zcozkjBaRSqTY5yVWiTaImEiO1ScU9Zi1%2FUL3oYS%2BS7icAeCiI%2F2xmCwQhL4ZWge6oyMVMry0CCxriFpyNX9jxSDAYbt%2BA233%2FrpyKA1lyE%2FEyM%2FSrt4JQ1mjmkwu7YSxbRvxaXAlnxTe6bBkt3ABMC%2BXf3ye16iyQrj3EcrmDNm4Lzm3sK9Y71Z3jC3%2BeldO37hwVvpOQNoYdAH8ADR9Lrl7qEZYLK5mFLJwGBEcYYqLzUp1%2FRc4XcBKy3AdYJvl8mSIMZ3z507%2BAQIyOiLMGQLpJw2F1KxawvfaJ6L6NuNTwgkbobfw%2FaGGMMXYQORiY5F%2Fy0RpUlDqnc%2BDdmaGFdScGOWeYxSexHr%2B3n9mEbeRc%2B3%2FGObbLP%2FJMj712vU0APKv3IIVBzONzqIh8d%2F8gYhrwe2JXxaEitQs62AEPOkBCye2sxfQ1KGOvwUHmGH%2BPv9QNdDRlCoeYY5DQYF9xDKc8Ebk3c2ksUcCSj4zaBHiqgMuC8DqFRqv5UDXJcniUQff3d%2Bcla%2Fg6FV7xvW%2FEdJasPLwOxID2dpPldN3n9tRhXwCf52enXP7OyMQFLsQRwN6CVrMYQXTFFqCLMx%2FAri4k0vrCVoJwZ2D9aRSGuH1rjC7dt4G%2Bbo7o9By3zmwZMCeXO%2FWzcQaUVMl2lP%2BP0QWyCgWtwEOIlJLa4Gm3rDIFDI1QVUzyPKeT34DwKX5ghmtZWOqpMzcQcHrG7i52RX7Rl1saRTjS%2BwTL2%2FximyA3Ej5iOpnfXAkUlDa3NthbPJCD8AUZtVYjvZhL2N2z5xSlvAkUZNOZsd6UKc4P6peIY3HhlL52O%2B%2F7s6a%2FA82dbKelePacgc7WdhgHyL0Mx5E3Hgw143h1n0l8MxcgMwIBiEp%2BlbGl8tyEPfnt0yQSNAza1Xt3J3px4%2FYTn2aHUvZZmi3QzGyVJifQHdWHTGfjnUQc7dWjvPR7IhqmHuDjVrOd7FbICSVXzon53F7krDTaufhiiBA7sA3JmBJ9X0ZL9lFigK59zuZ59sY%2FtDsaskSEw0irZYGURbEVyo1ZTa5XQBuJJjaybYPefPyIwPAqqHUcUGzXig8DmDoOJWn3uvfu0Mg5ZUS8iS7LyXLWHGG2lFMTLvO6ND%2FAuofFRMIRB9tGde6v4CoHn0AO%2Fj1rMPitbYb8tj7NfAPQDyQf3Zj6nH899iAS%2Fgu8%2Fd7QejPMcCSZSWt%2FPEAbxN%2F7XgCsQYn5DbR4KUbBg13OXHXjvXPC549jJAejzGjuWWOmRRtxmJac5LZdMwAZVZ6tn8JX6MILtws8d1Xvwe7wosrxgrHEy4cDkrGk3bYBHzoTUefUo3xdTNuE3VuVp3Bivz6Km%2F7gtwZ6Iko4KBzfp0JQ7ioPvBNWiGpjIswZMxG%2Flhh8WBLndWNg6fDShqYP4yGPqG64vSbKp2ScFyCUgtmdeK4xZIZfRCQc2O5SUJoc2JBrDeXYbC6KqfvVRtgk7XQesNgKkegDzxP8ISGRPjCVF35%2FVxrHxHioimn5hAc3prQ6kDbRCi%2B%2FdUmT5zNbxf58OSr2Qm3xfa8TGVKOH03ZcetiGWLfBUOCbg6NQcu43mqo0dPxBCdW0cHy1A0hgnJ5c7Of82Cnxz9PqEkghTQh8GsxTjV9JL%2FMum2udBn%2BoD3yT3AvBjp6L8MPqZHTCsyY0VACnhLFDSF6pzK6R%2B0mQjnUaDfIDDTfKQ5CGb73FbiYJ3XleWpg16C6GnsIepqYeMRYASGlbn0kfFYZUXnUFEIwYuXXWF4iWaDgquWxxIkVxMi6RMw2nbkx7vP%2FM1DvvMK0ubR%2Fi76%2FQK5LWXkMbQKKv9ECjIt8lG9az%2FD1hlYww5kjYP3XFOJa975CqFizJZmQnnwYnglREl0g%2FX1hxrj3YyFUQfXDxG%2F%2BJ8%2BzEGiq5nVWjLj4n%2BUcK4Fxcxpul3NWLJO8GpztLPxhKHUrU%2FdckXejsNLa3yst%2Bu20iFqZ72E1mqq8ypLYT0Luj30za1fiX0jI7xpgxX7IGgDqJT%2FV7Lknngdq8pT3ZYSRya17k6Jc%2FWrNDn0a%2BE36X4cLTHqrPJ1HmAV8lFGrYSgMiADmInly%2FXnZcGkaJen9%2Buv1w%2BW9DbJuqnAoeTO0Ay0RMeMgOtKAv9Sd2XWKm3gDNaTn9fJp1D88TVc%2B5sFW1exYEt9MFLwevacJQ1KnmAxWqHzqpb3%2FoYG2nHssZmqLBrNM1%2FWKCjeNg8C%2Bzvf0wn1Fh9DXxh7v4s6WZ7QImcEp8nVUub7jVdx6v5IuHjjzbAdorzIFajKrqGglzV8gEHpjUNT2KnULR7%2B8ZpxdAM5H6xON2Yk8NHqaO%2B1hNz1U6nUs1n0HtsUxER6Q9%2FtTMQr7Y6S%2FUXihmlCPEy1QW6s8Pn02d4ivImU3ifcc21%2FCRtFTspKOhiyHPl35VmV%2BIgKWgylH%2F2hT85ydbQB7j0ws7bQd3ibxA9p6tbZcKk7gwaW8sjhwXrggeR2RKfiG%2Fwxz%2BIItzZvQzYW3xLqbLvh03kgd7mTZfcE6nEN3Uh42qwhl24iqz%2FF2ZxXyyBgG465uITrl8ToafnaWZwmDxhmgCAMHZOOJTlaDmbye1mgqdiWsAy5ttMMXPqj3tWJ1E1YSjVugvzZvm1%2FWQbDSCVIqKWH0ndUcbOfTpYm1ksPtWm0%2B7Das4JGJKxET%2B1ow049e6%2Ftcf9M%2BtZKQn%2BBT2edutnDB3%2Br0Io9znWx8ik5GFqVABD08wEqiVPacsuOuqr7Eu3dPRTgxbUI0voZQ99W%2Fa2pkyqtasAhMVD6yCTKm%2F%2F0s%2FhMSs8JD7%2BK7kTJU54tpyTY64UfUhOs4Hswgn3YuGrOn7VpATqt3QBSFS%2F%2FIdNN32w3kgOip%2Fe2PukNox44O1Rx6yLOjZ7U2a5%2F4DyW8Cwlp1fXYBH7jqMbfdt%2BlZQ3WC6q8JvpSBhMZlxsIV95xCHUfwZs1t%2FdgGWrvsw2H6c6fzodFCbsJwLeyummgp6v058uM2ZaqSMq46X7ydhSPa%2BRBKZW6%2Ft3iV4hWqwhCPKuuWu3Z76CAvUMnosTnArqDo6cQOneEaJOB2TPLqxNiyZCc5N8Rb%2FxLf9VVGC629yAZ0frtdAj38XfSlGotgUEEsQ%2Baxmow3M%2F6TZ9SZidJM9IijXSMN%2BHHkTWqqfW%2FI9QPLcJ%2B048ivMRUJfauiHIsShAjPdYeB%2BDUNd%2BTQ6n7OnU3Ue9%2FJC4d46w4TZrflU%2FnJ8zUH7%2BIBEIYOiuLF7st9Oa8iho9ZMrMYy81D1W95f%2BQOCYEVUIfVf4Q43rUdMrfgsW52c0LZ1rtn8EZ4lcdmWEH1k9yCJZQaN97z5CYfkMu5ESEx0L4ecN1oJm%2Bt0StsGvZd73o%2BahaSJ9MLUOvzvilRdlhmwVy5w%2FLAmc%2FlqrSVCJEFk96pwTBD6Z8XE6OKfviu%2BeY3yiYdvQFv83fSqf2RX0CTf548UBttc0hFG767Oq0qEWgObbwbv5yRapxQjO8k%2BVrmaJZcoyJ94O8yd7ZXfQ4eGB8MjLiS%2FgHRP5cHbWcWRjFoP13aFdHdccUbrcWm3HORKJnoa%2FIsl70BW1wn4m6AZaUaJgyGtRfkxgJbnG5sUxMX84U7nlnSu0eoMzVATC8GF3QGlL8sKxwe9J6mkI57eBvrK58ecaJoPgZ%2BUAT1reI7v4tOvZB9pgT7E5j3V8CTZAjzwpWcrnKwJEMhuZcu5iXJY76rnJIXtVCPmnSgoJ4Ixqg89ul%2FNCzihhUggVRQViNxF5X0hqkP9hOwaV4wn0STQGzVX9aPBHIk7ULmlGTvt6hQ65MwfMzH2ZtpBgz7lT6Ga3Xy95PBichf9drYcBaZP0mxxvm8UaKI%2BY5IH%2FdKUFNX7%2FiZsLZQd5dN%2BWjBgd8qPF%2FbOoEZmhs8sgFAV33tC3430dwNQArTTokf%2FhpcfHKyCnPDwgdhq965u0jTyhgZ89bTUe9r0VVxPiyBneGXDQrHX1Co0pTTUbDhayalerxxwpHMm4f5KiTFWZ7zrMwOGyKPHPjsR%2FpA7QZqbqidBk9DGzIhK60mdvAHB80OQbaMGvwbcizOVXvEMqYKl%2BLU5CWtMNsYAVpDYKb%2F6bvoyQ92FYtpw5X%2FVoRhoTSw68qBHb8n21IAVc2TzJkatrr59WhB3ZEJtdgDFdQ%2B1p529EvgRl4ODa5ncS9AKhILWndDpotve2AnbyWJbQyo1KWurDQfeU8QyMSVN43wQekN%2BfHX5qrux%2FWz0Vg5Ir9TnaglMHZgH39DBnW2ILmaB2yU%2B5%2FB9%2BuyriVaYjiSPwyxQvvTTBczQeV2r2Bs%2BZA4yM2Y5%2FUpLqzQ%2BpwqmAoLgJhIVCxgbHkD1nh7BV47aEz5AYBCM2RFuIPh9W1NzbRX1%2FN8Uf3yqLCQs2nN7t6Ia%2B5HqvSYg9pk%2FICd0Ta7S3NetpwCFglPAHu8tkbJhKddGHAbqISovthsykRwhFXDyRUhZER3sTRJU76DkIYb%2BAGF%2FZGHqh43FurYkfHjpkEaqn9bVKQSYDbhoPu9aaK1w8Vr9delTlM7Tv8otyDXGjyuWK3yXyg%2BHPPNXTyNIoOFazw%2BjEtVjDmHMS18qY%2BEBBFJPnyBcQb0GO0BXo%2B3sUW%2FiMd%2Bzm2ox%2BEj8aMcxMCRMhraV73tc0MI1rtlM1u9NZjV2xgkKkixwK6kGQ91noO%2FbYJjSlRg8NKx6VEtwQq8D5zeiZrscXD1BBE2QsWVMPK5ta95fi6EXLbFCFRN8wrywA5UTLuCLr1RBHFhZyVy0%2Fi3tXlFfibRgnV6O%2BpaC4ub2VW9vXj%2BWPlJiTQYlper%2BC2G7MyulBOtB5Z91vMXypQHUMASX9jE2YfgSrgTuS6WY5r9fji2zmhZ%2Bsj%2FjgL0ox4eYhue39NffINaAWG216njojo1vzeGz1ZuR002qNEDfkkEkzTnIuJ%2FcfAjen32f9kH41%2FOkLsbxziiA55xw4Z0CYDqVqxmwqq4QTK3pI9olCEO5gOEl%2Bci5PArwK6D6ONWfyyYdF%2BeaBmwLbMnzyo4TdykFzTVuX2XMzpGjwbcNWvj1ibQKTVA4etPiPtbeRQcLzGJKaPx1s3lJZkMN%2FYhNigK28qmZgvariUoULHifmvZ5sNn7Ak3%2FteBqq9lUpGwhk4Vf9yYyg8%2FJw25QZAXOYH%2FjXZ3g%2B%2FgtefNS%2B8Mm6HxCPLA68BAfzJ2yerI1M6BJ2U5e9ou%2B%2BfXVmC%2F5GVi1tx8SnsznUWbIP04ne7MgSp5shVORsVbapCFJsioh2fHyPE1sk4mU3jlKyzKL1QJJsAD%2BxObk6nnq1pP88%2FsxceVTiVfoZnqtdavyt0XyBYTpcUMtSph%2FOtY%2FD9AjAGmkPal6YbIaSTwVVPP7ZOtV3RFsju9M%2FcvEUPr8ojKoO1%2Bqzr8YBiCw5MyAalMhvLCucJpfh6FJ%2FejX6Re2AP%2BeMg0pTzeaJDKBk3KAG9Y%2FlojQqBYIDPlzPEPLVsQ1d9DTe6Zau4rpJbHbgqtsyxNlZPC4RM3pIf3DTLNXvxbfJ6YySGvmWNYWaCXCKmhIX8%2BKnOichCYvm2643X6A8yaqpzpY4CvGatf553VDFeeS%2BABZqEZ6ZRuftnI%2BwtXx6MUO91x%2FngpI%2FRgeEacDuetDJ3pwcAjTKdOVRzehUKhRrCGgrrkr3ZnuJKlejp1fy3CG1xwB1JcEGM7SP2tl62rQzqVzmoBisEAW7SWgGrjMHdecGI%2BkPt6HTffwzMRha3xJvekwv65svWKA3ndPDD9FiOAil4%2FVqLWUJtKygXrHCHWeXyV1eoZYsveQ0UkZqyFkk2Q2CyBjNV7HiIwXuvFYxEXDhxcVyVOiFZWqNUIIyG97rYhkuOtuFk5uNsn%2F077j8lJN%2B2K4dAHnhGm%2F07eUW3xiW%2F7HvcV2PbW%2FngkcOZ8pRCvythkG66lOdCROpYrk93QK2dc9rtILPJyWLVjrPl99hIZWIaUsFAK9gjM3xu6Oglh0%2FQQbOKtm0cCbVPxi2LVEnn%2BUxyNUdIdx8MPlGSjZlwGS%2FqutiYKO1V1JO8iEaNN974qlsg85pt7fnyr5pDeZ5JEviAHvHe0%2BVdj1Jc5AzpKc62kYfJQA6g5Iv4fBvofuz6LPrbc5ZRZXGBFKPcUcT4ot6tz3m7jIA%2B8E2eyyXDskGN4AKg%2FteTTGVk%2B7j1NxFleaSB0G0Z5sxJZflRamirK8403Kfpd5G%2BNg%2FxutDjHuN%2F5lNwFJxOKOqpemnWC2lvD5TmvAtIsiOMWtB0K8F4EsV%2BW%2FHAadp27eobVwWl%2FbI0gHAdbrhKh%2BwA8DZG7ZBVHj8cIxhcyBtk4znvKCCWostzeeTcG4JI7VYD9Mei%2FpZx9C41GW93SJV4vuRABO64WOyuRBGqDtSSEVswaQst4Uc9ti6l%2FZ9pMFD4N7hjsIttwYpfxy5NO4pn%2Fez2MhLxrCdUnZTs3sSrGEPQHgzaLR9SBWazvZARDk0sEzUDqJ73agOfuJHrUOiEdNDBhWyz8rKscX4mgtOoGgO%2FKgZPUJeUocenPsZknpfximqIx3dQIrH6UKYwL4KjfJf08%2BCHTiLj%2FfYuxJYSUKzl0rAOimvsD0uMnl4o%2B%2BhGBjLvp9BQ9%2FVEn1lOTEFTOpcrnpjXG6aRGqi7s8KjKG%2FtdF8unodmBEAMRNEQTiL0iCSgwTmAyKFigcy1M1zA2intCObSZZ%2FrQFyOGB4oBtpUpYXerGQ35JfTpTUK3nRmUAbQI9l%2BATtEUlZtgyT2wc%2Bu%2BNmhOVp%2FHlfo9cO9Ok4URAHfw2pn9USHKTEjO3rU6vFS0HQl8r8bOmVP0bOVifcaPme7rhe7gcqo8cmhBZL0dBLweXk%2FJuZX6gYbI%2FpMvBy8iDkQQvu%2BmGfYbFhSuA4DwMBpT9aYzrt%2FrAjwDR1yy8i5M01m6gQ39lqzPUfMpfWgrUP8JEmmLRkDu5rcpX2wFAEaHGeC0JyZySCPzTWqT3EWAXIcfsvBAQuY1%2FxAFHxqiv8K81HxxCLaaCm6a7QniwuKuXqjxE%2BT4CKFrrlEoNPKII1EDYSvvgk4ImbffyzMwigV3mLJH9MwD2gSciJ1YvjY9ZNEckFheJR3Yydlr7fzb77w9ho1o2I9CWn5Icqxl1KqQvChmH1PXKqsv%2FimkTOD9GHRlqzsnsgDyYrT8vkGvO4QeZZdryG6NvWqmLMvWIaXzfT%2FmPAq4Ev07gOHvJnNr46UabvNUSroUBk6QZfMcUGX4pedW3k7arlpfXEbIIkYi5bTzDNMlk2wn%2FeZZtS%2BrffOqJMDqBN4%2FDob6%2F7dfyVsurWkiVgQXU%2B8wKVUcdLJ8rIJd%2F13dAsxnC3ob1BoAM%2BXi42iMo1TTT1pgDMjyVnMGvz0ICOCUtEbmwfqRcet2T77lJhEHptJqlLq50Qb9UhGSXJwzOpniXAA6oE2PpOCWpCK%2FM%2FkgJjbVPSt3o0E5aipzuAXOd8JLlx9a4V3psSA7lnwmN2n4sHCpb1ydnNtJeU8LTMQQ9hXQoAi128tjgZWX6it9z4pJ0b%2FWuya1AW7OVOYF5cIzI7ENPKWupAx9HcGTInLVccmbPb0zTupldApfrY7oPhxdvSUXl1cSzuWaYaRYiK1Fw%2BCxI4basmVbH%2F0jbGc7bf8VqqQgrxja5JYPRjHu7uqnX1iRV1BSdmCcZFcW58OIZm4VQotWkMkYTgZx9y%2Bv6VDa%2FU%2FRYnYvLF3D%2FNr%2FjESFdae%2BGV4jVQt%2B%2FqkY51A7KZZsG21ygw5sPbJu4duV%2F8JZ4QnXQ6ntArywCVR3zzwxbJd%2BnPoHQdxhLy%2BKEKRP1Gpcig%2BrazubWnhItXsxGKj7ticyFqQ%2FQSDGjiTw9eLYyL5FM3eJFA6VT1rhKTKZKflojPnUqxMG3IyNrqBihCO12s6YKrGlgQPcQAdciwseEWogb44xuLGOPD%2BmbjYqZ8uQWce4c5x0t5zW6HI8vGYsdAtygHEu4LlobgWJCdLdPrGw%2FdIUsBtlfbuSxtganyZXAxFfC36Uw80a2I07D4hKBWiZKXZtcxz1nDoWSvAZ5urxyUIrMR2HJdRNbi0qW457bMyHI%2F4qVaT6AwmSDXj%2F8YeUxvDUjDapKIceqttoPbC%2Fb0vXFRV4upteth3uwEbUJlXUbKUanbv3Gemka7iBJ0lnzyaLY%2Fo9aMStIOJZxttHEfAT9CM9bOPHXYo1V4lnKVJg4Pmfq0PcFlfxxo%2FnPHm6lUszh3GQPN5g%2BGMXQ%2BpXz1cPP2fQFcljm0NdGN2a7thb64zsD5mRHdmEWsutwa%2Bc1uV78ROzjXEt8TZ8EcZnQnKmykReQ6PsluUEG2wR8NRX7EnuP6eRyu249UXkNBwicYfuqjXgZvy4vpxfFcP5jmxkhp84j0pTDYfYBN5kL8%2FLq3ZOnvPSxr2A1JSSoqPfUjlierSul9VS5ccBIxxOONUIJ%2BAh5HVjyq%2FV%2FDwnEpI8rmVmocRsloT%2Fjx%2BE2kGXMdSv4xM7kbmbrI4O8gHhxar1Gm9yb4JyWV0Va53ub4URbOfDDAAkc0G3iN8p6P3eC%2FLBqRZhzXZbvO2ETlO4hd%2BlIeQPAsz0b%2FCMh72hHH8%2FjXdn%2BGiNpDAz%2Bwpkvt%2B221OoW8AfHW170zlDIk7ti%2B3YGEm6FNxWv3ygkG%2FcyUwdt%2BsFh6EyIS8noBliOG6Q24onyEvgmckRojTwFxRN3GFedSaoDaVFaHHoKsv0WxJSJyMYaqeCGAGvWuEQXCu%2BOTfL53UWKU6E17ZnGQzCvAkWtxp%2FnkJn7qhfBTvI58biH9BnjWHRloNDUsyaFKxtQi8Te75stT4wXl3entj%2FNlOZP3FUaLY%2B0hqrLNY84sVqsMtnwflolubzsdGzbXEYQF4K8WycweMEljOKdyNzFPmP6s%2BtcVd916zjjX%2B3zHVJBllu5KdSx%2B72y5OGqqHKi0cYqJqWO6lqnNK36jTqGaypMDeyjFPYsAlq6CduiUXeyANkD6jh3PeHlGl14AONG7mDcLa3L9wQx%2FSFpGowgUKkGyasI5XHUu2AiN%2FGvvs7U7TIN%2BwqhBqKY0IxHVhjWEfv%2FTfT13B1StwpLgP%2FVdV2jg6Rig4yp0vIbjZGJNxl2jgE6Me1c8MbjtAS%2ByflcX6A98rL0OlwI1DuMr7yfEWE%2B6lEXTUz7ODrmQ9MgVDSldGJi824JDS8aI2xuU%2Fdei%2B2DJ6wlCSHfjLrvz7wexVqSQgbY2o9j9bCbYjX%2BXeIhAc4ldHB28zE5H%2FNLlGsujQjR1jUzMA6KbsBHEtFYClMFjWVMtknJ2SDWtjiZfIBOD8NtjeDOraj0LQB20LMy1VgSXKaebtbK9rxO8i%2FjYj5I9UP3t5ZbHt7G38WjcS8%2FqW3q9aY2vxgv%2FcYCfNSrSx8XQ%2Bd2PkLoJu0pIQDqN13jAxzX5Ts6Eikb0IgYMzC6rC%2Fx8FgfcT5cOGxNdWjq74DHl0N%2FDvWgqwY2X6Ye3gYycvVGT4hSS1OBP5PnoGIBueG9EKIPBf4BfSRl73BSLgaLq9bXy8lzQijdCzLhjX8saRtSJoDHgsb5OvLgBxe1VV5d%2BfJEH3oqcTK9%2B8s2iSTs6LFSH%2BIFSzPnxqXy1DKM98XEJCnqpwivQmnKEmjh%2BAyslGdOSe4sMZf85j9cWRPMlTqLDCAi%2BzVQak80BheGOUUpobo7A411FtiOawi2FmCFuoaKoFVALm2GiMfmMiYtQaVMob6bo%2FmkrDrb9srICpvgTMarAe9V2Wdy4rBpNPT%2FqtCMnJDS2qDQXSRA4ZQVEBdXaUKROC466hVAerV7d5KU%2F2lW2MGYt9GOlyCMYt32d805h%2FIR2kcXSFdqLsROPoywZMWHlJ4rmXeKAJ3Ar4w0bhudj%2BKpmFVvwdQXN8PKPs5m15R%2BlPimQmusAW7RCFBV6jq5orcJh3q6XAKZ84sz1KSwL9rXf6e69lryGHaZp%2FHFkkg1jM5MADCzmA8p%2FFOlseZdNLyTHLvWgXd3pekkhgFNGnL7G2XpDdQXT%2B%2FaH2ER5rIi%2BKw2a3oOcU%2B8mJBw8F0WYdxzpZG4Chg9c5FXnY%2Biv%2Bg8YQ7KyG1NWPYwguvnajvFd6NYXPIn4JO2pSDyfezY8BiD27FgqHXrReo240dJ13WHd8NnNYrq4JzI3zqxrldQujJxMi5XAOszaYlxpa3HsLy0gPT5SE7PENHWUeopuG0hsyI5rMODVRrUtyFusiD%2BIbRx0YoAPuD75i1vS3rchy0V57ee0jAGNmovbfLBogcpD%2FwJORJIJBZsWs%2Fb1h3J58BApgTXAje6w6Uzv1GiI4aItQ5gnReU20XV13u8opfMpjPOnMhMf2pY6yRORzkY9aI32nFlvKeKP0jv0HRf6OlET6HlQ%2F3gdaFh1VlizqgxjzVdOd3b%2Fkv3K3JcXvcnU9ZQ%2FHOM9%2FYJnPZhOvbEcojYGwEUo8DRkjCCbHfQGSlhCEwfWBOLdO7dMrJwif7gKAHsKPMT%2BawdqKUBKpmHp%2BDLQl92J6tmP8KQASR9T%2Be5u4sEurmeUS%2Bab0a%2FiRpbTxslNhS8znh1dKLQDJNYge33Mg%2B4ThgUqMD0HBosEb%2F%2B6%2FW%2BpEdOpo0lJTb1Wo7dUoFmy7eyoRCiWSNVrwvLh%2F7EZT6z8TFEl%2Bnm2ULlDO9ZjHpgFraFv%2FcgyU427H2u8JOsUK7K2N1q4%2FNSmTb3wnSv9uUHOQvXEX7kFnP6IrB%2FAoT%2BL4u3jIcaynjetgf5t5QnftG%2BrZ1x4kJhjKDWa0jz3eJj8giRkW%2BuHGQfEzHGlGud3XNmwMsxK%2BFhuiAZzqvf1Ubf1gALGE5NsZA1IFoIMg6K%2FMXIkpiJX4%3D&__VIEWSTATEGENERATOR=67237148&__EVENTVALIDATION=Yo%2BoRZzFbqhUMGjW2VmBFoIQr1POVkyRTf0JlGJPF4gVnfpWYxjE6JF5O7NMq2A2G0%2FefStWyYT3Uy8oDoAMhLCMWCOhcetgDme8YOqWuy4WFAWjTOfeVl%2FGZ5ZQpomVVqTrgtmv8TXO1juRyiIbPHSwi1UbsYJD5Lo%2FNEf4OqdWwjTbEgIuI8Mb4rR9JG0Woxb5pzM6hGp2N9uun%2FJWmjGbujq9zCrT3u%2B143CwZYJJkbtVmutS2KV%2FleaCZkz8rgQAKG9dGYi4L3xx8e3oiPDpjy08YVGMU3viU2JowLflojR2dMSS%2FoFKFqDToEBNflMQYQUxlcf0W%2F%2Bx4VAwSRnCdgJJOURbs%2BNN9Ne3A6H2QlTfgKn7ZsCDNvnrMc%2BN6DKWIXUWyv2yeiK9mc7Sg5%2BjrofudOZym9jwdQajMUXgbVdIG47LoBehkoWj4gTI%2FIxTtnpqAfqI0nqbynpzv22R665do3pR1Q5t3dHpZUdv67ZbkvO5NJpFJO%2B%2B9lTzIJ7%2FeukOgn0pFX3O1Gn8wM97zBlORrtjf7f6PEWsEYchCPCEGZMRy67nXDM04krz1IV3f8JfuatXqh4iv8CN%2F0mjIMrlFguWxJxq%2B%2ByGvmQAuBvC3pAG%2BTeQ%2F1BuK0I3rs7NhsLRQq7grVu%2BsE1sosiNBxuN3glxjvAqU7eLd%2FaQ7hY8sXKdTE3mrU1DNrPqGiXGLQAqrcF1Z0FOVQGyJfvo7t48ElzZvBov1XWMzASr2D62NSA2Iqj26rXRrg10rWtdanSPI%2BA9gr11jKvckrtUvpuQdtQlqf6G2XdayLdsisFRgmepKNdrmGgHi%2FiZB15b4JHKeVg9aFANN8ntdmWK7xEmr%2BixFcKHmECGDMtiOFO0GJyWS%2FoDqJB4bc7L4y6aLgWJ5SlJ%2F9o3IJaRmUZ0tfdywFlRfT9xkyNq%2BtFww9O7rJSGNPJvSnNuAyIjzqdkInHCBP4We8fEqEHOOWezy7254S7tNW0Djbcr1nZI9f8U%2BWHjMC08I23xumwOvnFTuTb99gGafX8VtrWuWw%2BdnZHzdM8oWPYqydFvi6d%2BWpFJTunFF9%2F%2BKkQrHqeaklsCJTDtSe2%2FCMHIGa1fwJatei11I9wu1rRgzqHvcBHjN44IO1dKElVmZqbZ5O78gg6ePDe%2FSZhBVjR3Trb5exvuBvFROlfWLLVEFrqF43M6ty%2B6vSARx4iMcVqQR2j6&ctl00%24ContentPlaceHolder1%24txtip=" + textBoxIP.Text + "&ctl00%24ContentPlaceHolder1%24dlyear=" + currentTime.Year.ToString() + "&ctl00%24ContentPlaceHolder1%24dlmonth=" + currentTime.Month.ToString() + "&ctl00%24ContentPlaceHolder1%24dlday=" + currentTime.Day.ToString() + "&ctl00%24ContentPlaceHolder1%24dlcunit=1048576&ctl00%24ContentPlaceHolder1%24btnview=%E6%AA%A2%E8%A6%9624%E5%B0%8F%E6%99%82%E6%B5%81%E9%87%8F";

                // 将提交的字符串数据转换成字节数组
                byte[] postData = Encoding.ASCII.GetBytes(postString);

                // 设置提交的相关参数
                HttpWebRequest request = WebRequest.Create(URI) as HttpWebRequest;
                request.Method = "POST";
                request.KeepAlive = false;
                request.ContentType = "application/x-www-form-urlencoded";
                request.CookieContainer = cookieContainer;
                request.ContentLength = postData.Length;

                // 提交请求数据
                System.IO.Stream outputStream = request.GetRequestStream();
                outputStream.Write(postData, 0, postData.Length);
                outputStream.Close();

                // 接收返回的页面
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                System.IO.Stream responseStream = response.GetResponseStream();
                System.IO.StreamReader reader = new System.IO.StreamReader(responseStream, Encoding.GetEncoding("UTF-8"));
                string srcString = reader.ReadToEnd();

                string[] stringSeparators_source = new string[] { "<td>", "</tr>","<table>" };
                string[] result = srcString.Split(stringSeparators_source, StringSplitOptions.RemoveEmptyEntries);
                download = "下載：" + result[3].Replace(" ", "").Replace("</td>", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
                upload = "上傳：" + result[4].Replace(" ", "").Replace("</td>", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
                total = "總計：" + result[5].Replace(" ", "").Replace("</td>", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
                label_download.Text = download;
                label_upload.Text = upload;
                label_total.Text = total;

                string totalNum_s = result[4].Replace(" ", "").Replace("</td>", "").Replace("</tr><tr>", "").Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace("(M)", "").Replace(",","");
                int totalNum = 0;
                if (totalNum_s != "") {
                    try {
                        totalNum = Int32.Parse(totalNum_s);
                    }
                    catch (Exception e)
                    {

                    }
                }
                if(totalNum > 4500)
                {
                    notifyIcon.ShowBalloonTip(1000, "流量警告！", "目前流量" + total, ToolTipIcon.Info);
                }


            }
            catch (WebException we)
            {
                string msg = we.Message;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RunWhenOn = checkBox1.Checked;
            Properties.Settings.Default.Save();
            SetAutoRun(Application.ExecutablePath, checkBox1.Checked);
        }
        //reference = http://fanli7.net/a/bianchengyuyan/csharp/2011/0921/129713.html
        public static void SetAutoRun(string fileName, bool isAutoRun)
        {
            RegistryKey reg = null;
            try
            {
                if (!File.Exists(fileName))
                {
                    throw new Exception("木有這個文件，搞什麼搞");
                }
                string name = fileName.Substring(fileName.LastIndexOf(@"\") + 1);
                reg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (reg == null)
                    reg = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                if (isAutoRun)
                {
                    reg.SetValue(name, fileName);
                    MessageBox.Show("下次開機將自動啟動！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    reg.SetValue(name, false);
            }
            catch (Exception ex) { }
            finally
            {
                if (reg != null)
                {
                    reg.Close();
                }
            }
        }
        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            getData();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://network.ntust.edu.tw/");
        }
    }
}
