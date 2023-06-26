using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using System.Xml;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Windows;
using System.Net.NetworkInformation;

namespace TizenXamlApp1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        AbsoluteLayout absolutelayout = new AbsoluteLayout();
        private int standx = 800;
        private int standy = 200;
        ImageButton back_button = new ImageButton() { Source = "back.png" };
        Image[] backgrounds = new Image[4];
        int background_idx = 0;

        public MainPage()
        {
            InitializeComponent();
            Home();

        }

        public void Home()
        {
            absolutelayout.Children.Clear();

            backgrounds[0] = new Image() { Source = "1.jfif" };
            backgrounds[1] = new Image() { Source = "2.jfif" };
            backgrounds[2] = new Image() { Source = "3.jpg" };
            backgrounds[3] = new Image() { Source = "4.jfif" };

            for (int i = 0; i < 4; i++)
            {
                AbsoluteLayout.SetLayoutBounds(backgrounds[i], new Rectangle(0, 0, 1920, 1080));
            }

            absolutelayout.Children.Add(backgrounds[background_idx]);

            Label clockLabel;
            clockLabel = new Label { FontSize = 80 };
            AbsoluteLayout.SetLayoutBounds(clockLabel, new Rectangle(160, 100, 400, 300));
            absolutelayout.Children.Add(clockLabel);

            Image clock = new Image() { Source = "analog_clock.png"};
            Image secondhand = new Image() { Source = "second_hand.png", };
            Image minutehand = new Image() { Source = "minute_hand.png" };
            Image hourhand = new Image() { Source = "hour_hand.png" };
            AbsoluteLayout.SetLayoutBounds(clock, new Rectangle(140, 410, 540, 540));
            AbsoluteLayout.SetLayoutBounds(secondhand, new Rectangle(310, 430, 200, 500));
            AbsoluteLayout.SetLayoutBounds(minutehand, new Rectangle(310, 430, 200, 500));
            AbsoluteLayout.SetLayoutBounds(hourhand, new Rectangle(310, 430, 200, 500));
            absolutelayout.Children.Add(clock);
            absolutelayout.Children.Add(hourhand);
            absolutelayout.Children.Add(minutehand);
            absolutelayout.Children.Add(secondhand);

            // 주기적으로 시간 업데이트
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                // 현재 시간 가져오기
                DateTime currentTime = DateTime.UtcNow;
                TimeZoneInfo koreaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time"); // 대한민국 시간대(Time Zone)
                DateTime koreaCurrentTime = TimeZoneInfo.ConvertTimeFromUtc(currentTime, koreaTimeZone); // 대한민국 시간으로 변환

                hourhand.Rotation = 30 * (koreaCurrentTime.Hour % 12)+(0.5 * currentTime.Minute);
                minutehand.Rotation = 6 * koreaCurrentTime.Minute;
                secondhand.Rotation = 6 * koreaCurrentTime.Second;                

                // 시간을 문자열로 변환
                string timeString = koreaCurrentTime.ToString("yyyy.MM.dd\nHH:mm:ss");

                // 시계 표시 업데이트
                Device.BeginInvokeOnMainThread(() =>
                {
                    clockLabel.Text = timeString;
                });

                return true;
            });

            ImageButton[] images = new ImageButton[3];
            Label[] intro = new Label[3];

            images[0] = new ImageButton { Source = "Weather.png" };
            AbsoluteLayout.SetLayoutBounds(images[0], new Rectangle(standx, standy, 300, 300));
            images[1] = new ImageButton { Source ="SH.png" };
            AbsoluteLayout.SetLayoutBounds(images[1], new Rectangle(standx + 350 ,standy, 300, 300));
            images[2] = new ImageButton { Source = "Settings.png" };
            AbsoluteLayout.SetLayoutBounds(images[2], new Rectangle(standx + 700, standy, 300, 300));

            for (int i = 0; i < 3; i++)
            {
                images[i].Clicked += ImageButton_Clicked;
                images[i].Focused += ImageButton_Focused;
                images[i].Unfocused += ImageButton_Unfocused;
                absolutelayout.Children.Add(images[i]);
            }

            Content = absolutelayout;

            void ImageButton_Clicked(object sender, EventArgs e)
            {
                for (int i = 0; i < 3; i++)
                {
                    images[i].Focused -= ImageButton_Focused;
                    images[i].Unfocused -= ImageButton_Unfocused;
                }

                var imageButton = (ImageButton)sender;

                if (imageButton.Equals(images[0]))
                {
                    Weather();
                }
                else if (imageButton.Equals(images[1]))
                {
                    SmartHome();
                }
                else if (imageButton.Equals(images[2]))
                {
                    Settings();
                }
            }

            void ImageButton_Focused(object sender, FocusEventArgs e)
            {
                var imageButton = (ImageButton)sender;

                imageButton.Scale = 1.3; // 이미지 크기 확대

                intro[0] = new Label { Text = "-Weather-\n현재 전국 도시들의 날씨 정보를 보여줍니다. 지도와 날씨 아이콘을 이용하여 현재 날씨 정보를 효과적으로 전달합니다.", FontSize = 50, BackgroundColor = Color.Gray, Opacity = 0.7 };
                intro[1] = new Label { Text = "-Smart-Home-\n현재 집에 있는 SMARTHOME 가구의 상태를 나타냅니다.리모컨 조작을 통한 개별 제어 및 버튼을 통한 전체 제어가 가능합니다.", FontSize = 50, BackgroundColor = Color.Gray, Opacity = 0.7 };
                intro[2] = new Label { Text = "-Settings-\n설정창입니다. 홈 배경화면 스타일을 사용자의 조작에 따라 설정할 수 있습니다.", FontSize = 50, BackgroundColor = Color.Gray, Opacity = 0.7 };

                for (int i = 0; i < 3; i++)
                {
                    AbsoluteLayout.SetLayoutBounds(intro[i], new Rectangle(800, 550, 1000, 350));
                    if (imageButton.Equals(images[i]))
                    {
                        absolutelayout.Children.Add(intro[i]);
                        Content = absolutelayout;
                    }
                }
            }

            void ImageButton_Unfocused(object sender, FocusEventArgs e)
            {
                var imageButton = (ImageButton)sender;

                imageButton.Scale = 1.0; // 이미지 크기 복원

                for (int i = 0; i < 3; i++)
                {
                    if (imageButton.Equals(images[i]))
                    {
                        absolutelayout.Children.Remove(intro[i]);
                        Content = absolutelayout;
                    }
                }
            }
        }

        void GoBack()
        {
            AbsoluteLayout.SetLayoutBounds(back_button, new Rectangle(0, 0, 100, 100));
            absolutelayout.Children.Add(back_button);
            Content = absolutelayout;

            back_button.Clicked += Button_Clicked;
            back_button.Focused += Button_Focused;
            back_button.Unfocused += Button_Unfocused;

            void Button_Clicked(object sender, EventArgs e)
            {
                Home();
            }

            void Button_Focused(object sender, FocusEventArgs e)
            {
                var imageButton = (ImageButton)sender;

                imageButton.Scale = 1.3; // 이미지 크기 확대
            }

            void Button_Unfocused(object sender, FocusEventArgs e)
            {
                var imageButton = (ImageButton)sender;

                imageButton.Scale = 1.0;
            }
        }
        public async void Weather()
        {
            absolutelayout.Children.Clear();

            GoBack();

            Label title = new Label() { Text = "Weather", FontSize = 100 };
            AbsoluteLayout.SetLayoutBounds(title, new Rectangle(50, 200, 600, 200));
            absolutelayout.Children.Add(title);
            Label intro = new Label() { Text = "현재 전국 도시들의 날씨 정보를 보여줍니다. 지도와 날씨 아이콘을 이용하여 날씨 정보를 효과적으로 전달합니다.", FontSize = 50 };
            AbsoluteLayout.SetLayoutBounds(intro, new Rectangle(50, 500, 600, 600));
            absolutelayout.Children.Add(intro);

            int y = 200;
            Image map = new Image() { Source = "map.PNG" };
            AbsoluteLayout.SetLayoutBounds(map, new Rectangle(600, 50, 800, 950));
            absolutelayout.Children.Add(map);

            Label label = new Label() { FontSize = 100};
            AbsoluteLayout.SetLayoutBounds(label, new Rectangle(1300, 30, 500, 200));
            absolutelayout.Children.Add(label);

            string[] weatherstr = { "맑음", "흐림", "구름많음", "흐리고 비" };
            string[] imagestr = { "sunny.PNG", "cloudy.PNG", "cloudy.PNG", "rainy.PNG" };

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("http://www.kma.go.kr/weather/forecast/mid-term-rss3.jsp?stnId=108");

            if (!response.IsSuccessStatusCode)
            {
                label.Text = "서버에서 오류를 반환했습니다. 반환 코드={response.StatusCode}";
            }
            label.Text = "전국날씨";
            
            string content = await response.Content.ReadAsStringAsync();

            XmlDocument document = new XmlDocument();
            document.LoadXml(content);

            XmlNodeList nodes = document.DocumentElement.SelectNodes("descendant::location");          

            GetWeatherInfo("서울ㆍ인천ㆍ경기도", "서울");
            GetWeatherInfo("서울ㆍ인천ㆍ경기도", "인천");
            GetWeatherInfo("충청북도", "충주");
            GetWeatherInfo("전라북도", "전주");
            GetWeatherInfo("제주도", "제주");
            GetWeatherInfo_2();

            async void GetWeatherInfo(string _province, string _city)
            {
                foreach (XmlNode node in nodes)
                {
                    var provinceNode = node.SelectSingleNode("province");
                    var cityNode = node.SelectSingleNode("city");

                    if (provinceNode == null || cityNode == null) continue;

                    if (provinceNode.InnerText == _province && cityNode.InnerText == _city)
                    {
                        var _nodes = node.SelectNodes("descendant::data");
                        foreach (XmlNode _node in _nodes)
                        {
                            var weatherNode = _node.SelectSingleNode("wf");
                            var minNode = _node.SelectSingleNode("tmn");
                            var maxNode = _node.SelectSingleNode("tmx");

                            if (weatherNode == null || minNode == null || maxNode == null) continue;

                            for (int i = 0; i < 4; i++)
                            {
                                if (weatherNode.InnerText == weatherstr[i])
                                {
                                    Image image = new Image() { Source = imagestr[i] };
                                    if (_city == "서울")
                                    {
                                        AbsoluteLayout.SetLayoutBounds(image, new Rectangle(800, 200, 150, 150));
                                    }

                                    else if (_city == "인천")
                                    {
                                        AbsoluteLayout.SetLayoutBounds(image, new Rectangle(720, 250, 150, 150));
                                    }

                                    else if (_city == "충주")
                                    {
                                        AbsoluteLayout.SetLayoutBounds(image, new Rectangle(930, 420, 150, 150));
                                    }

                                    else if (_city == "전주")
                                    {
                                        AbsoluteLayout.SetLayoutBounds(image, new Rectangle(850, 550, 150, 150));
                                    }

                                    else if (_city == "제주")
                                    {
                                        AbsoluteLayout.SetLayoutBounds(image, new Rectangle(800, 850, 150, 150));
                                    }
                                    Label max_temp = new Label() { Text = $"{maxNode.InnerText} /", TextColor = Color.Red };
                                    Label min_temp = new Label() { Text = $" {minNode.InnerText}", TextColor = Color.Blue };
                                    AbsoluteLayout.SetLayoutBounds(max_temp, new Rectangle(AbsoluteLayout.GetLayoutBounds(image).X + 30, AbsoluteLayout.GetLayoutBounds(image).Y - 30, 70, 100));
                                    AbsoluteLayout.SetLayoutBounds(min_temp, new Rectangle(AbsoluteLayout.GetLayoutBounds(image).X + 90, AbsoluteLayout.GetLayoutBounds(image).Y - 30, 70, 100));
                                    absolutelayout.Children.Add(max_temp);
                                    absolutelayout.Children.Add(min_temp);
                                    absolutelayout.Children.Add(image);
                                }
                            }
                            Content = absolutelayout;
                            break;

                        }
                    }

                }
                Content = absolutelayout;
            }

            async void GetWeatherInfo_2()
            {
                foreach (XmlNode node in nodes)
                {
                    var cityNode = node.SelectSingleNode("city");

                    if (cityNode == null) continue;
                    { 
                        var _nodes = node.SelectNodes("descendant::data");
                        foreach (XmlNode _node in _nodes)
                        {
                            var weatherNode = _node.SelectSingleNode("wf");
                            var minNode = _node.SelectSingleNode("tmn");
                            var maxNode = _node.SelectSingleNode("tmx");
                            var reliablility = _node.SelectSingleNode("rnSt");

                            if (weatherNode == null || minNode == null || maxNode == null) continue;

                            Label weatherinfo = new Label() { Text = $"{cityNode.InnerText}: {weatherNode.InnerText}", FontSize=30};
                            Label weatherinfo_2 = new Label() { Text = $"{maxNode.InnerText} ", TextColor = Color.Red, FontSize = 30 };
                            Label weatherinfo_3 = new Label() { Text = $"{minNode.InnerText} ", TextColor = Color.Blue, FontSize = 30 };
                            Label weatherinfo_4 = new Label() { Text = $"확률: {reliablility.InnerText}%", FontSize = 30 };
                            AbsoluteLayout.SetLayoutBounds(weatherinfo, new Rectangle(1300, y, 400, 100));
                            AbsoluteLayout.SetLayoutBounds(weatherinfo_2, new Rectangle(1500, y, 100, 100));
                            AbsoluteLayout.SetLayoutBounds(weatherinfo_3, new Rectangle(1600, y , 100, 100));
                            AbsoluteLayout.SetLayoutBounds(weatherinfo_4, new Rectangle(1700, y += 70, 400, 200));
                            absolutelayout.Children.Add(weatherinfo);
                            absolutelayout.Children.Add(weatherinfo_2);
                            absolutelayout.Children.Add(weatherinfo_3);
                            absolutelayout.Children.Add(weatherinfo_4);
                            Content = absolutelayout;
                            break;
                        }
                    }
                }
            }
        }

        public void SmartHome()
        {
            absolutelayout.Children.Clear();

            GoBack();

            Image smarthome = new Image() { Source = "smarthome.png" };
            AbsoluteLayout.SetLayoutBounds(smarthome, new Rectangle(400, 0, 1920, 1080));
            absolutelayout.Children.Add(smarthome);
            ImageButton[] on = new ImageButton[5];
            ImageButton[] off = new ImageButton[5];
            Label title = new Label() { Text = "Smart Home", FontSize = 100 };
            AbsoluteLayout.SetLayoutBounds(title, new Rectangle(50, 200, 600, 200));
            absolutelayout.Children.Add(title);
            Label intro = new Label() { Text = "현재 집에 있는 SMARTHOME 가구의 상태를 나타냅니다\n리모컨 조작을 통한 개별 제어 및 버튼을 통한 전체 제어가 가능합니다", FontSize = 50 };
            AbsoluteLayout.SetLayoutBounds(intro, new Rectangle(50, 500, 600, 600));
            absolutelayout.Children.Add(intro);
            Button all_on_button = new Button() { Text = "Turn On All" };
            Button all_off_button = new Button() { Text = "Turn Off All" };
            absolutelayout.Children.Add(all_on_button);
            absolutelayout.Children.Add(all_off_button);
            AbsoluteLayout.SetLayoutBounds(all_on_button, new Rectangle(50, 830, 300, 200));
            absolutelayout.Children.Add(title);
            AbsoluteLayout.SetLayoutBounds(all_off_button, new Rectangle(350, 830, 300, 200));
            absolutelayout.Children.Add(intro);


            for (int i = 0; i < 5; i++)
            {
                on[i] = new ImageButton() { Source = "on.jpg" };
                off[i] = new ImageButton() { Source = "off.jpg" };
            }
            AbsoluteLayout.SetLayoutBounds(on[0], new Rectangle(950, 100, 150, 150));
            AbsoluteLayout.SetLayoutBounds(on[1], new Rectangle(900, 350, 150, 150));
            AbsoluteLayout.SetLayoutBounds(on[2], new Rectangle(1250, 150, 150, 150));
            AbsoluteLayout.SetLayoutBounds(on[3], new Rectangle(1450, 850, 150, 150));
            AbsoluteLayout.SetLayoutBounds(on[4], new Rectangle(1750, 550, 150, 150));

            AbsoluteLayout.SetLayoutBounds(off[0], new Rectangle(950, 100, 150, 150));
            AbsoluteLayout.SetLayoutBounds(off[1], new Rectangle(900, 350, 150, 150));
            AbsoluteLayout.SetLayoutBounds(off[2], new Rectangle(1250, 150, 150, 150));
            AbsoluteLayout.SetLayoutBounds(off[3], new Rectangle(1450, 850, 150, 150));
            AbsoluteLayout.SetLayoutBounds(off[4], new Rectangle(1750, 550, 150, 150));

            for (int i = 0; i < 5; i++)
            {
                on[i].Clicked += ImageButton_Clicked;
                on[i].Focused += ImageButton_Focused;
                on[i].Unfocused += ImageButton_Unfocused;
                off[i].Clicked += ImageButton_Clicked;
                off[i].Focused += ImageButton_Focused;
                off[i].Unfocused += ImageButton_Unfocused;
                absolutelayout.Children.Add(on[i]);
            }

            all_on_button.Clicked += All_On;
            all_off_button.Clicked += All_Off;

            Content = absolutelayout;

            void All_On(object sender, EventArgs e)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (absolutelayout.Children.Contains(off[i]))
                    {
                        absolutelayout.Children.Remove(off[i]);
                    }
                    absolutelayout.Children.Add(on[i]);
                }
                Content = absolutelayout;
            }

            void All_Off(object sender, EventArgs e)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (absolutelayout.Children.Contains(on[i]))
                    {
                        absolutelayout.Children.Remove(on[i]);
                    }
                    absolutelayout.Children.Add(off[i]);
                }
                Content = absolutelayout;
            }

            void ImageButton_Clicked(object sender, EventArgs e)
            {
                var ImageButton = (ImageButton)sender;

                for (int i = 0; i < 5; i++)
                {
                    if (ImageButton.Equals(on[i]))
                    {
                        absolutelayout.Children.Remove(on[i]);
                        absolutelayout.Children.Add(off[i]);
                        break;
                    }
                    else if (ImageButton.Equals(off[i]))
                    {
                        absolutelayout.Children.Remove(off[i]);
                        absolutelayout.Children.Add(on[i]);
                    }
                }
                Content = absolutelayout;
            }

            void ImageButton_Focused(object sender, FocusEventArgs e)
            {
                var imageButton = (ImageButton)sender;

                imageButton.Scale = 1.5; // 이미지 크기 확대
            }

            void ImageButton_Unfocused(object sender, FocusEventArgs e)
            {
                var imageButton = (ImageButton)sender;

                imageButton.Scale = 1.0; // 이미지 크기 복원
            }
        }

        void Settings()
        {
            absolutelayout.Children.Clear();

            GoBack();

            Label title = new Label() { Text = "Settings", FontSize = 100 };
            AbsoluteLayout.SetLayoutBounds(title, new Rectangle(50, 200, 600, 200));
            absolutelayout.Children.Add(title);
            Label intro = new Label() { Text = "설정창입니다. 홈 배경화면 스타일을 사용자가 조작에 따라 설정할 수 있습니다. 리모컨으로 원하는 배경화면을 선택하세요.", FontSize = 50 };
            AbsoluteLayout.SetLayoutBounds(intro, new Rectangle(50, 500, 600, 600));
            absolutelayout.Children.Add(intro);

            ImageButton[] backgrounds = new ImageButton[4];
            backgrounds[0] = new ImageButton() { Source = "1.jfif" };
            backgrounds[1] = new ImageButton() { Source = "2.jfif" };
            backgrounds[2] = new ImageButton() { Source = "3.jpg" };
            backgrounds[3] = new ImageButton() { Source = "4.jfif" };

            AbsoluteLayout.SetLayoutBounds(backgrounds[0], new Rectangle(650, 200, 600, 400));
            AbsoluteLayout.SetLayoutBounds(backgrounds[1], new Rectangle(1300, 200, 600, 400));
            AbsoluteLayout.SetLayoutBounds(backgrounds[2], new Rectangle(650, 600, 600, 400));
            AbsoluteLayout.SetLayoutBounds(backgrounds[3], new Rectangle(1300, 600, 600, 400));

            for (int i = 0; i < 4; i++)
            {
                backgrounds[i].Clicked += Clicked;
                backgrounds[i].Focused += Focused;
                backgrounds[i].Unfocused += Unfocused;
                absolutelayout.Children.Add(backgrounds[i]);
            }

            Content = absolutelayout;

            void Clicked(object sender, EventArgs e)
            {              
                var image = (ImageButton)sender;

                for (int i = 0; i < 4; i++)
                {
                    if (sender.Equals(backgrounds[i]))
                    {
                        background_idx = i;
                        Home();
                    }
                }
            }

            void Focused(object sender, FocusEventArgs e)
            {
                var imageButton = (ImageButton)sender;

                imageButton.Scale = 1.2; // 이미지 크기 확대
            }

            void Unfocused(object sender, FocusEventArgs e)
            {
                var imageButton = (ImageButton)sender;

                imageButton.Scale = 1.0; // 이미지 크기 복원
            }
        }
       
    }
}

