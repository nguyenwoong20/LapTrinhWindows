using LapTrinhWindows.Models;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace LapTrinhWindows.Buoi6.QuanLySinhVien
{
    public partial class Form1 : Form
    {
        private Model1 dbContext;
        private String studentID;

        public Form1()
        {
            InitializeComponent();
            dbContext = new Model1();
            LoadFaculties();
            LoadStudents();
        }

      
        private void LoadFaculties()
        {
            var faculties = dbContext.Faculty.ToList();
            cboFaculty.DataSource = faculties;
            cboFaculty.DisplayMember = "FacultyName";
            cboFaculty.ValueMember = "FacultyID";
        }

       
        private void LoadStudents()
        {
            var students = dbContext.Student
                .Select(s => new
                {
                    s.StudentID,
                    s.FullName,
                    FacultyName = s.Faculty.FacultyName, // Lấy tên Khoa từ liên kết
                    AverageScore = (float)s.AverageScore
                }).ToList();

            dgvStudents.DataSource = students;
        }   
        private void ClearInputs()
        {
            txtStudentID.Clear();
            txtFullName.Clear();
            txtAverageScore.Clear();
            cboFaculty.SelectedIndex = -1;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F2) 
            {
                btnQuanLyKhoa.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.F)) 
            {
                btnTimKiem.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnQuanLyKhoa_Click(object sender, EventArgs e)
        {
            var formQuanLyKhoa = new QuanLyKhoa.QuanLyKhoa();
            // Đăng ký sự kiện
            formQuanLyKhoa.DataUpdated += () =>
            {
                LoadFaculties(); // Tải lại danh sách khoa
            };
            formQuanLyKhoa.ShowDialog();
        }
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            var formTimKiem = new FormTimKiem();
            formTimKiem.ShowDialog();
        }

     
        private void dgvStudents_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStudents.CurrentRow != null)
            {
                txtStudentID.Text = dgvStudents.CurrentRow.Cells["StudentID"].Value.ToString();
                txtFullName.Text = dgvStudents.CurrentRow.Cells["FullName"].Value.ToString();
                txtAverageScore.Text = dgvStudents.CurrentRow.Cells["AverageScore"].Value.ToString();
                cboFaculty.Text = dgvStudents.CurrentRow.Cells["FacultyName"].Value.ToString();
            }
        }

        private void dgvStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvStudents.Rows.Count)
            {
              
                DataGridViewRow selectedRow = dgvStudents.Rows[e.RowIndex];
             
                studentID = selectedRow.Cells["StudentID"].Value?.ToString();
               
                string fullName = selectedRow.Cells["FullName"].Value?.ToString();
                string facultyName = selectedRow.Cells["FacultyName"].Value?.ToString();
                float averageScore = float.Parse(selectedRow.Cells["AverageScore"].Value?.ToString() ?? "0");
                txtStudentID.Text = studentID;
                txtFullName.Text = fullName;   
                txtAverageScore.Text = averageScore.ToString();
                cboFaculty.Text = facultyName;


            }
        }

        private void btnThemSua_Click(object sender, EventArgs e)
        {
            string studentID = txtStudentID.Text.Trim();
    string fullName = txtFullName.Text.Trim();
    float averageScore;

    if (!float.TryParse(txtAverageScore.Text, out averageScore))
    {
        MessageBox.Show("Điểm không hợp lệ!");
        return;
    }

    int facultyID = (int)cboFaculty.SelectedValue;

    // Tìm sinh viên trong database
    var student = dbContext.Student.FirstOrDefault(s => s.StudentID == studentID);

    if (student == null)
    {
        // Thêm mới nếu không tìm thấy
        student = new Student
        {
            StudentID = studentID,
            FullName = fullName,
            FacultyID = facultyID,
            AverageScore = averageScore
        };
        dbContext.Student.Add(student);
        MessageBox.Show("Thêm sinh viên thành công!");
    }
    else
    {
        // Cập nhật nếu tìm thấy
        student.FullName = fullName;
        student.FacultyID = facultyID;
        student.AverageScore = averageScore;
        MessageBox.Show("Cập nhật sinh viên thành công!");
    }

    try
    {
        dbContext.SaveChanges();
        LoadStudents(); // Tải lại danh sách sinh viên
        ClearInputs();  // Xóa nội dung form nhập liệu
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Lỗi khi lưu dữ liệu: {ex.Message}", "Lỗi");
    }
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            try
            {
                var studentID = txtStudentID.Text;
                var student = dbContext.Student.FirstOrDefault(s => s.StudentID == studentID);

                if (student != null)
                {
                    dbContext.Student.Remove(student);
                    dbContext.SaveChanges();
                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo");
                    LoadStudents();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên để xóa!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa sinh viên: {ex.Message}", "Lỗi");
            }
        }
    }
}
